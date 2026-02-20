using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Extensions;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Contracts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(IContractService contractService, ILogger<ContractsController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        /// <summary>
        /// Get all Contracts
        /// </summary>
        /// <returns>List of Contracts</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAllContracts()
        {
            try
            {
                var contracts = await _contractService.GetAllContractsAsync();

                // Manufacturer role restriction: limit results to assigned manufacturer IDs
                var roleClaims = User?.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList() ?? new List<string>();
                if (roleClaims.Contains("Manufacturer"))
                {
                    var claim = User?.FindFirst("manufacturer_ids")?.Value;
                    var allowedIds = new HashSet<int>();
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(claim))
                        {
                            var ids = System.Text.Json.JsonSerializer.Deserialize<List<int>>(claim);
                            if (ids != null) allowedIds = new HashSet<int>(ids);
                        }
                    }
                    catch { }
                    contracts = contracts.Where(c => c.ManufacturerId.HasValue && allowedIds.Contains(c.ManufacturerId.Value));
                }

                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contract by ID
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Contract details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetContractById(int id)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    return NotFound(new { message = "Contract not found" });
                }

                // Manufacturer role restriction
                var roleClaims = User?.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList() ?? new List<string>();
                if (roleClaims.Contains("Manufacturer") && contract.ManufacturerId.HasValue)
                {
                    var claim = User?.FindFirst("manufacturer_ids")?.Value;
                    var allowedIds = new HashSet<int>();
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(claim))
                        {
                            var ids = System.Text.Json.JsonSerializer.Deserialize<List<int>>(claim);
                            if (ids != null) allowedIds = new HashSet<int>(ids);
                        }
                    }
                    catch { }
                    if (!allowedIds.Contains(contract.ManufacturerId.Value))
                    {
                        return Forbid();
                    }
                }

                // Debug log for products count
                _logger.LogInformation("GetContractById {Id}: Products count = {Count}", id, contract.Products?.Count ?? 0);
                if (contract.Products?.Any() == true)
                {
                    _logger.LogInformation("GetContractById {Id}: Products = {Products}", id,
                        string.Join(", ", contract.Products.Select(p => $"{p.Id}:{p.Name}")));
                }

                return Ok(contract);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Contract", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Contract
        /// </summary>
        /// <param name="createContractDto">Contract creation data</param>
        /// <returns>Created Contract</returns>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<ContractDto>> CreateContract([FromBody] CreateContractDto createContractDto)
        {
            _logger.LogInformation("CreateContract hit at {Time}", DateTime.UtcNow);
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var roles = string.Join(',', User.Claims.Where(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase)).Select(c => c.Value));
                _logger.LogInformation("[ContractsController] Entry userId={userId}, roles=[{roles}]", userId, roles);

                var raw = HttpContext.Items["RawBody"] as string;
                if (!string.IsNullOrEmpty(raw)) _logger.LogDebug("Raw JSON: {Raw}", raw);

                _logger.LogDebug("[ContractsController] DTO summary: name='{name}', productIds={pCount}, prices={plCount}",
                    createContractDto?.Name,
                    createContractDto?.ProductIds?.Count ?? 0,
                    createContractDto?.Prices?.Count ?? 0);

                // Log incoming price types at Information level for troubleshooting
                if (createContractDto?.Prices != null)
                {
                    foreach (var pr in createContractDto.Prices.Take(50))
                    {
                        _logger.LogInformation("[ContractsController] Incoming price item: productId={productId}, priceType='{priceType}'", pr.ProductId, pr.PriceType);
                    }
                }

                if (HttpContext.Items.TryGetValue("HasDuplicateKeys", out var hasDup) && hasDup is true)
                {
                    if (HttpContext.Items.TryGetValue("DuplicateKeys", out var dupKeysObj) && dupKeysObj is List<string> dupKeys)
                    {
                        // Filter duplicates: keep only top-level duplicates
                        var uniqueKeys = dupKeys
                            .GroupBy(k => k.ToLowerInvariant())
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

                        if (uniqueKeys.Any())
                        {
                            return BadRequest(new
                            {
                                message = "Duplicate JSON keys detected at root level.",
                                duplicateKeys = uniqueKeys
                            });
                        }
                    }
                }

                if (!ModelState.IsValid || createContractDto == null)
                {
                    return ValidationProblem(ModelState);
                }

                var createdBy = HttpContext.GetAuditPrincipal();
                var contract = await _contractService.CreateContractAsync(createContractDto, createdBy);
                if (contract == null || contract.Id <= 0)
                {
                    _logger.LogInformation("[ContractsController] No eligible items; contract not created for name='{name}'", createContractDto?.Name);
                    return NoContent();
                }
                return CreatedAtAction(nameof(GetContractById), new { id = contract.Id }, contract);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error during CreateContract");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                return StatusCode(500, new { message = "An error occurred while creating the Contract" });
            }
        }

        /// <summary>
        /// Update an existing Contract
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <param name="updateContractDto">Contract update data</param>
        /// <returns>Updated Contract</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ContractDto>> UpdateContract(int id, [FromBody] UpdateContractDto updateContractDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = HttpContext.GetAuditPrincipal();
                var contract = await _contractService.UpdateContractAsync(id, updateContractDto, modifiedBy);
                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Contract", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a Contract
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContract(int id)
        {
            try
            {
                var result = await _contractService.DeleteContractAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Contract not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Contract", error = ex.Message });
            }
        }


        /// <summary>
        /// Get Contracts by manufacturer ID
        /// </summary>
        /// <param name="manufacturerId">Manufacturer ID</param>
        /// <returns>List of Contracts for the manufacturer</returns>
        [HttpGet("manufacturer/{manufacturerId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByManufacturerId(int manufacturerId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByManufacturerIdAsync(manufacturerId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contracts by distributor ID
        /// </summary>
        /// <param name="distributorId">Distributor ID</param>
        /// <returns>List of Contracts for the distributor</returns>
        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByDistributorId(int distributorId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByDistributorIdAsync(distributorId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contracts by OpCo ID
        /// </summary>
        /// <param name="opCoId">OpCo ID</param>
        /// <returns>List of Contracts for the OpCo</returns>
        [HttpGet("opco/{opCoId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByOpCoId(int opCoId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByOpCoIdAsync(opCoId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contracts by industry ID
        /// </summary>
        /// <param name="industryId">Industry ID</param>
        /// <returns>List of Contracts for the industry</returns>
        [HttpGet("industry/{industryId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByIndustryId(int industryId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByIndustryIdAsync(industryId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contracts by status
        /// </summary>
        /// <param name="status">Status (1=Draft, 2=Active, 3=Expired, 4=Terminated)</param>
        /// <returns>List of Contracts with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByStatus(int status)
        {
            try
            {
                var contracts = await _contractService.GetContractsByStatusAsync(status);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get suspended Contracts
        /// </summary>
        /// <returns>List of suspended Contracts</returns>
        [HttpGet("suspended")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetSuspendedContracts()
        {
            try
            {
                var contracts = await _contractService.GetSuspendedContractsAsync();
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving suspended Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Contracts for performance
        /// </summary>
        /// <returns>List of Contracts marked for performance</returns>
        [HttpGet("performance")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsForPerformance()
        {
            try
            {
                var contracts = await _contractService.GetContractsForPerformanceAsync();
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Contracts for performance", error = ex.Message });
            }
        }

        /// <summary>
        /// Get expiring Contracts
        /// </summary>
        /// <param name="beforeDate">Date before which contracts expire</param>
        /// <returns>List of expiring Contracts</returns>
        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetExpiringContracts([FromQuery] DateTime beforeDate)
        {
            try
            {
                var contracts = await _contractService.GetExpiringContractsAsync(beforeDate);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving expiring Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get expiring Contracts that have not been pushed to the proposal workflow
        /// </summary>
        /// <param name="daysThreshold">Number of days threshold (default: 90)</param>
        /// <returns>List of expiring Contracts without associated proposals</returns>
        [HttpGet("expiring-without-proposals")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetExpiringContractsWithoutProposals([FromQuery] int daysThreshold = 90)
        {
            try
            {
                var contracts = await _contractService.GetExpiringContractsWithoutProposalsAsync(daysThreshold);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving expiring Contracts without proposals", error = ex.Message });
            }
        }

        /// <summary>
        /// Suspend a Contract
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Success status</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPatch("{id}/suspend")]
        public async Task<ActionResult> SuspendContract(int id)
        {
            try
            {
                var modifiedBy = HttpContext.GetAuditPrincipal();
                var result = await _contractService.SuspendContractAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Contract not found" });
                }
                return Ok(new { message = "Contract suspended successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while suspending the Contract", error = ex.Message });
            }
        }

        /// <summary>
        /// Unsuspend a Contract
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Success status</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPatch("{id}/unsuspend")]
        public async Task<ActionResult> UnsuspendContract(int id)
        {
            try
            {
                var modifiedBy = HttpContext.GetAuditPrincipal();
                var result = await _contractService.UnsuspendContractAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Contract not found" });
                }
                return Ok(new { message = "Contract unsuspended successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while unsuspending the Contract", error = ex.Message });
            }
        }

        /// <summary>
        /// Send Contract to performance
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/send-to-performance")]
        public async Task<ActionResult> SendToPerformance(int id)
        {
            try
            {
                var modifiedBy = HttpContext.GetAuditPrincipal();
                var result = await _contractService.SendToPerformanceAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Contract not found" });
                }
                return Ok(new { message = "Contract sent to performance successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending Contract to performance", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove Contract from performance
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/remove-from-performance")]
        public async Task<ActionResult> RemoveFromPerformance(int id)
        {
            try
            {
                var modifiedBy = HttpContext.GetAuditPrincipal();
                var result = await _contractService.RemoveFromPerformanceAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Contract not found" });
                }
                return Ok(new { message = "Contract removed from performance successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing Contract from performance", error = ex.Message });
            }
        }

        /// <summary>
        /// List versions for a contract
        /// </summary>
        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<ContractVersionDto>>> GetContractVersions(int id)
        {
            try
            {
                var versions = await _contractService.GetVersionsAsync(id);
                return Ok(versions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving contract versions", error = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific contract version
        /// </summary>
        [HttpGet("{id}/versions/{versionId}")]
        public async Task<ActionResult<ContractVersionDto>> GetContractVersion(int id, int versionId)
        {
            try
            {
                var version = await _contractService.GetVersionAsync(id, versionId);
                if (version == null) return NotFound(new { message = "Contract version not found" });
                return Ok(version);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving contract version", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new contract version with prices (Duplicate/Amend flow)
        /// </summary>
        [HttpPost("{id}/versions")]
        public async Task<ActionResult<ContractVersionDto>> CreateContractVersion(int id, [FromBody] CreateContractVersionRequest request)
        {
            try
            {
                var createdBy = HttpContext.GetAuditPrincipal();
                var version = await _contractService.CreateVersionAsync(id, request, createdBy);
                return Ok(version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating contract version", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing contract version
        /// </summary>
        [HttpPut("{id}/versions/{versionId}")]
        public async Task<ActionResult<ContractVersionDto>> UpdateContractVersion(int id, int versionId, [FromBody] UpdateContractVersionRequest request)
        {
            try
            {
                var modifiedBy = HttpContext.GetAuditPrincipal();
                var version = await _contractService.UpdateVersionAsync(id, versionId, request, modifiedBy);
                return Ok(version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating contract version", error = ex.Message });
            }
        }

        /// <summary>
        /// Clone an existing version (by version number) and create a new editable version
        /// </summary>
        [HttpPost("{id}/cloneVersion/{versionNo}")]
        public async Task<ActionResult<ContractVersionDto>> CloneVersion(int id, int versionNo)
        {
            try
            {
                var createdBy = HttpContext.GetAuditPrincipal();
                var version = await _contractService.CloneVersionByNumberAsync(id, versionNo, createdBy);
                return Ok(version);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cloning contract version", error = ex.Message });
            }
        }

        /// <summary>
        /// Compare two versions and return structured differences
        /// </summary>
        [HttpGet("compare")]
        public async Task<ActionResult<object>> Compare([FromQuery] int contractId, [FromQuery] int versionA, [FromQuery] int versionB)
        {
            try
            {
                var result = await _contractService.CompareVersionsAsync(contractId, versionA, versionB);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while comparing contract versions", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new version of a Contract (legacy endpoint)
        /// </summary>
        /// <param name="id">Contract ID</param>
        /// <param name="changeReason">Reason for the version change</param>
        /// <returns>Updated Contract with new version</returns>
        [HttpPost("{id}/new-version")]
        public async Task<ActionResult<ContractDto>> CreateNewVersion(int id, [FromBody] string changeReason)
        {
            try
            {
                var createdBy = HttpContext.GetAuditPrincipal();
                var contract = await _contractService.CreateNewVersionAsync(id, changeReason, createdBy);
                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating new Contract version", error = ex.Message });
            }
        }

        /// <summary>
        /// Search Contracts with pagination and filtering
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="manufacturerId">Filter by manufacturer ID</param>
        /// <param name="status">Filter by status</param>
        /// <param name="distributorId">Filter by distributor ID</param>
        /// <param name="industryId">Filter by industry ID</param>
        /// <param name="isSuspended">Filter by suspension status</param>
        /// <param name="startDate">Filter by start date (contracts starting on or after this date)</param>
        /// <param name="endDate">Filter by end date (contracts ending on or before this date)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Contracts</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchContracts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? manufacturerId = null,
            [FromQuery] int? status = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] int? industryId = null,
            [FromQuery] bool? isSuspended = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Manufacturer role restriction: limit results to assigned manufacturer IDs
                IEnumerable<int>? allowedManufacturerIds = null;
                var roleClaims = User?.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList() ?? new List<string>();
                if (roleClaims.Contains("Manufacturer"))
                {
                    var claim = User?.FindFirst("manufacturer_ids")?.Value;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(claim))
                        {
                            var ids = System.Text.Json.JsonSerializer.Deserialize<List<int>>(claim);
                            allowedManufacturerIds = ids ?? new List<int>();
                        }
                        else
                        {
                            allowedManufacturerIds = new List<int>();
                        }
                    }
                    catch
                    {
                        allowedManufacturerIds = new List<int>();
                    }
                }

                var (contracts, totalCount) = await _contractService.SearchContractsAsync(searchTerm, manufacturerId, status, distributorId, industryId, isSuspended, startDate, endDate, page, pageSize, allowedManufacturerIds);

                return Ok(new
                {
                    data = contracts,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Send email notifications for contracts expiring soon without proposals
        /// </summary>
        /// <param name="daysThreshold">Number of days threshold (default: 90)</param>
        /// <returns>Number of notification emails sent</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPost("send-expiry-notifications")]
        public async Task<ActionResult> SendExpiryNotifications([FromQuery] int daysThreshold = 90)
        {
            try
            {
                var sentCount = await _contractService.SendExpiryNotificationsAsync(daysThreshold);
                return Ok(new { message = $"Sent {sentCount} expiry notification email(s)", sentCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contract expiry notifications");
                return StatusCode(500, new { message = "An error occurred while sending expiry notifications" });
            }
        }

        /// <summary>
        /// Get dashboard statistics (real counts from database)
        /// </summary>
        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _contractService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics", error = ex.Message });
            }
        }
    }
}
