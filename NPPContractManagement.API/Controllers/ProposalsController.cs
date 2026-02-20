using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.DTOs.Proposals;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Extensions;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/proposals")]
    [Authorize]
    public class ProposalsController : ControllerBase
    {
        private readonly IProposalService _service;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProposalsController> _logger;
        private readonly IUserService _userService;
        private readonly IUserManufacturerRepository _userManufacturerRepository;
        private readonly IEmailService _emailService;
        private readonly IProposalProductExcelService _excelService;
        private readonly IConflictDetectionService _conflictDetectionService;

        public ProposalsController(
            IProposalService service,
            ApplicationDbContext db,
            ILogger<ProposalsController> logger,
            IUserService userService,
            IUserManufacturerRepository userManufacturerRepository,
            IEmailService emailService,
            IProposalProductExcelService excelService,
            IConflictDetectionService conflictDetectionService)
        {
            _service = service;
            _db = db;
            _logger = logger;
            _userService = userService;
            _userManufacturerRepository = userManufacturerRepository;
            _emailService = emailService;
            _excelService = excelService;
            _conflictDetectionService = conflictDetectionService;
        }

        private bool IsAdminOrNpp()
        {
            static IEnumerable<string> Norm(IEnumerable<string?> s) => s.Select(v => v?.Trim().ToLowerInvariant()).Where(v => !string.IsNullOrWhiteSpace(v))!;
            var synonyms = new HashSet<string>(new[] { "admin", "administrator", "system administrator", "system admin", "sysadmin", "npp", "npp user" });

            var fromClaimTypes = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var fromRole = User.FindAll("role").Select(c => c.Value);
            var fromRoles = User.FindAll("roles").Select(c => c.Value);
            var all = Norm(fromClaimTypes.Concat(fromRole).Concat(fromRoles)).ToList();

            var expanded = new List<string>();
            foreach (var v in all)
            {
                if (v.StartsWith("[") && v.EndsWith("]"))
                {
                    try
                    {
                        var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(v) ?? Array.Empty<string>();
                        expanded.AddRange(Norm(arr));
                        continue;
                    }
                    catch { }
                }
                expanded.Add(v);
            }

            return expanded.Any(x => synonyms.Contains(x));
        }


        private bool IsAdminUser()
        {
            static IEnumerable<string> Norm(IEnumerable<string?> s) => s.Select(v => v?.Trim().ToLowerInvariant()).Where(v => !string.IsNullOrWhiteSpace(v))!;
            var synonyms = new HashSet<string>(new[] { "admin", "administrator", "system administrator", "system admin", "sysadmin", "contract manager", "npp", "npp user" });

            var fromClaimTypes = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            var fromRole = User.FindAll("role").Select(c => c.Value);
            var fromRoles = User.FindAll("roles").Select(c => c.Value);
            var all = Norm(fromClaimTypes.Concat(fromRole).Concat(fromRoles)).ToList();

            var expanded = new List<string>();
            foreach (var v in all)
            {
                if (v.StartsWith("[") && v.EndsWith("]"))
                {
                    try
                    {
                        var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(v) ?? Array.Empty<string>();
                        expanded.AddRange(Norm(arr));
                        continue;
                    }
                    catch { }
                }
                expanded.Add(v);
            }

            return expanded.Any(x => synonyms.Contains(x));
        }

        private IEnumerable<int> GetManufacturerIdsFromClaims()
        {
            var results = new List<int>();
            try
            {
                // Prefer explicit claims if present
                var single = User.FindFirst("manufacturer_id")?.Value;
                var plural = User.FindFirst("manufacturer_ids")?.Value; // may be JSON array or CSV or single

                void AddIfNumeric(string? val)
                {
                    if (string.IsNullOrWhiteSpace(val)) return;
                    if (int.TryParse(val, out var n)) results.Add(n);
                }

                if (!string.IsNullOrWhiteSpace(plural))
                {
                    var t = plural.Trim();
                    if (t.StartsWith("[") && t.EndsWith("]"))
                    {
                        try
                        {
                            var arr = System.Text.Json.JsonSerializer.Deserialize<List<int>>(t);
                            if (arr != null) results.AddRange(arr);
                        }
                        catch { }
                    }
                    else if (t.Contains(','))
                    {
                        foreach (var part in t.Split(',')) AddIfNumeric(part.Trim());
                    }
                    else
                    {
                        AddIfNumeric(t);
                    }
                }

                AddIfNumeric(single);
            }
            catch { }
            return results.Distinct();
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? proposalStatusId = null,
            [FromQuery] int? proposalTypeId = null,
            [FromQuery] int? manufacturerId = null,
            [FromQuery] DateTime? startDateFrom = null,
            [FromQuery] DateTime? startDateTo = null,
            [FromQuery] DateTime? endDateFrom = null,
            [FromQuery] DateTime? endDateTo = null,
            [FromQuery] DateTime? createdDateFrom = null,
            [FromQuery] DateTime? createdDateTo = null,
            [FromQuery] int? idFrom = null,
            [FromQuery] int? idTo = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = null)
        {
            IEnumerable<int>? manufacturerIds = null;
            var isAdminOrNpp = IsAdminOrNpp();
            if (!isAdminOrNpp)
            {
                var ids = GetManufacturerIdsFromClaims().ToArray();
                manufacturerIds = ids.Length > 0 ? ids : null;
            }

            var (items, total) = await _service.QueryProposalsAsync(
                search, page, pageSize, manufacturerIds,
                proposalStatusId, proposalTypeId, manufacturerId,
                startDateFrom, startDateTo, endDateFrom, endDateTo,
                createdDateFrom, createdDateTo, idFrom, idTo,
                sortBy, sortDirection);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var sample = items.FirstOrDefault();
                var statuses = items.Select(i => i.ProposalStatusId).Distinct().OrderBy(x => x).ToArray();
                _logger.LogDebug("[Ctrl] GetAll role={role}, mIds=[{ids}], total={total}, batchCount={count}, sampleId={sid}, sampleStatusId={ssid}, statuses=[{statuses}], statusId={statusId}, typeId={typeId}, manId={manId}",
                    isAdminOrNpp ? "Admin/NPP" : "Manufacturer",
                    manufacturerIds == null ? "" : string.Join(',', manufacturerIds),
                    total, items.Count(), sample?.Id, sample?.ProposalStatusId, string.Join(',', statuses),
                    proposalStatusId, proposalTypeId, manufacturerId);
            }

            return Ok(new { data = items, totalCount = total, page, pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProposalDto>> GetById(int id)
        {
            var item = await _service.GetProposalByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ProposalDto>> Create([FromBody] ProposalCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdBy = HttpContext.GetAuditPrincipal();

            var isAdminOrNpp = IsAdminOrNpp();
            int? requestedStatusId = null;

            // Backend enforcement: Admin/NPP (including synonyms) default status to Requested
            if (isAdminOrNpp)
            {
                var requested = await _db.ProposalStatuses.FirstOrDefaultAsync(s => s.Name == "Requested");
                if (requested != null)
                {
                    dto.ProposalStatusId = requested.Id;
                    requestedStatusId = requested.Id;
                }
            }

            var result = await _service.CreateProposalAsync(dto, createdBy);

            // Spec: when an NPP user creates a new proposal request, send an email alert
            // to all active manufacturer-classed users assigned to the same manufacturer.
            if (isAdminOrNpp && requestedStatusId.HasValue && dto.ManufacturerId.HasValue && result.ProposalStatusId == requestedStatusId.Value)
            {
                await NotifyManufacturerUsersOfRequestedProposalAsync(result, dto.ManufacturerId.Value);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProposalDto>> Update(int id, [FromBody] ProposalUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var modifiedBy = HttpContext.GetAuditPrincipal();

            // Workflow enforcement: only admin users may change per-product statuses via this endpoint.
            // For non-admins, strip incoming statuses so the service can preserve previous values.
            if (!IsAdminUser())
            {
                foreach (var p in dto.Products ?? new List<NPPContractManagement.API.DTOs.Proposals.ProposalProductCreateDto>())
                {
                    p.ProductProposalStatusId = null;
                }
            }

            var result = await _service.UpdateProposalAsync(id, dto, modifiedBy);
            return Ok(result);
        }

        [HttpPost("{id}/clone")]
        public async Task<ActionResult<ProposalDto>> Clone(int id)
        {
            var createdBy = HttpContext.GetAuditPrincipal();
            var result = await _service.CloneProposalAsync(id, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult> Submit(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            _logger.LogInformation("[ProposalsController] Submit hit: proposalId={id}, userId={userId}", id, userId);
            var ok = await _service.SubmitProposalAsync(id, userId);
            return ok ? Ok(new { message = "Submitted" }) : BadRequest(new { message = "Failed" });
        }

        [HttpGet("{id}/conflicts")]
        public async Task<ActionResult<ProposalConflictResultDto>> GetConflicts(int id)
        {
            try
            {
                var result = await _conflictDetectionService.DetectConflictsAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/accept")]
        public async Task<ActionResult> Accept(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            _logger.LogInformation("[ProposalsController] Accept hit: proposalId={id}, userId={userId}", id, userId);

            // Run conflict detection before accepting
            try
            {
                var conflicts = await _conflictDetectionService.DetectConflictsAsync(id);
                if (conflicts.HasConflicts)
                {
                    _logger.LogWarning("[ProposalsController] Accept blocked for proposalId={id}: {count} conflicts found", id, conflicts.TotalConflictCount);
                    return BadRequest(new { message = "Cannot accept proposal: pricing conflicts exist with active contracts. Resolve all conflicts before accepting.", conflicts });
                }
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            var ok = await _service.AcceptProductsAsync(id, userId);
            return ok ? Ok(new { message = "Accepted" }) : BadRequest(new { message = "Failed" });
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult> Reject(int id, [FromBody] RejectProposalRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            _logger.LogInformation("[ProposalsController] Reject hit: proposalId={id}, userId={userId}, reason={reason}", id, userId, request?.RejectReason);
            var ok = await _service.RejectProposalAsync(id, userId, request?.RejectReason);
            return ok ? Ok(new { message = "Rejected" }) : BadRequest(new { message = "Failed" });
        }

        [HttpPost("batch")]
        public async Task<ActionResult<object>> Batch([FromBody] IEnumerable<ProposalCreateDto> items)
        {
            var createdBy = HttpContext.GetAuditPrincipal();

            // Backend enforcement for batch: default Admin/NPP items to Requested
            if (IsAdminOrNpp())
            {
                var requested = await _db.ProposalStatuses.FirstOrDefaultAsync(s => s.Name == "Requested");
                if (requested != null)
                {
                    foreach (var it in items)
                    {
                        it.ProposalStatusId = requested.Id;
                    }
                }
            }

            var count = await _service.BatchCreateAsync(items, createdBy);
            return Ok(new { created = count });
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistory(int id)
        {
            var items = await _db.ProposalStatusHistories
                .Include(h => h.FromStatus)
                .Include(h => h.ToStatus)
                .Where(h => h.ProposalId == id)
                .OrderByDescending(h => h.ChangedDate)
                .Select(h => new
                {
                    changedDate = h.ChangedDate,
                    changedBy = h.ChangedBy,
                    fromStatus = h.FromStatus != null ? h.FromStatus.Name : null,
                    toStatus = h.ToStatus != null ? h.ToStatus.Name : null,
                    comment = h.Comment
                })
                .ToListAsync();
            return Ok(items);
        }

        private async Task NotifyManufacturerUsersOfRequestedProposalAsync(ProposalDto proposal, int manufacturerId)
        {
            try
            {
                if (manufacturerId <= 0) return;

                // Search active users (limit high but bounded)
                var (users, _) = await _userService.SearchUsersAsync(string.Empty, true, 1, 5000, "FirstName", "asc");

                // Filter by role = Manufacturer
                var eligible = users.Where(u => u.Roles.Any(r => string.Equals(r.Name, "Manufacturer", StringComparison.OrdinalIgnoreCase)));

                // Intersect with users assigned to this manufacturer
                var allowedUserIds = await _userManufacturerRepository.GetUserIdsForManufacturerAsync(manufacturerId);
                var idSet = new HashSet<int>(allowedUserIds);
                var targets = eligible.Where(u => idSet.Contains(u.Id)).ToList();

                if (!targets.Any())
                {
                    _logger.LogInformation("[ProposalsController] No manufacturer-classed users found for proposalId={proposalId}, manufacturerId={manufacturerId}", proposal.Id, manufacturerId);
                    return;
                }

                foreach (var user in targets)
                {
                    var name = ($"{user.FirstName} {user.LastName}").Trim();
                    await _emailService.SendProposalRequestedEmailAsync(
                        user.Email,
                        name,
                        proposal.Title ?? string.Empty,
                        proposal.Id,
                        proposal.StartDate,
                        proposal.EndDate);
                }

                _logger.LogInformation("[ProposalsController] Sent proposal requested notifications for proposalId={proposalId} to {count} users for manufacturerId={manufacturerId}", proposal.Id, targets.Count, manufacturerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ProposalsController] Failed to send proposal requested notifications for proposalId={proposalId}, manufacturerId={manufacturerId}", proposal.Id, manufacturerId);
            }
        }

	        [HttpGet("{id}/product-history")]
	        public async Task<ActionResult<IEnumerable<object>>> GetProductEditHistory(int id)
	        {
	            var items = await _db.ProposalProductHistories
	                .Include(h => h.ProposalProduct)!
	                .ThenInclude(pp => pp!.Product)
	                .Where(h => h.ProposalProduct != null && h.ProposalProduct.ProposalId == id)
	                .OrderByDescending(h => h.ChangedDate)
	                .Select(h => new
	                {
	                    id = h.Id,
	                    changedDate = h.ChangedDate,
	                    changedBy = h.ChangedBy,
	                    changeType = h.ChangeType,
	                    productId = h.ProposalProduct!.ProductId,
	                    productName = h.ProposalProduct!.Product != null ? h.ProposalProduct.Product.Name : null,
	                    previousJson = h.PreviousJson,
	                    currentJson = h.CurrentJson
	                })
	                .ToListAsync();
	            return Ok(items);
	        }

        /// <summary>
        /// Export proposal products/pricing to Excel
        /// </summary>
        [HttpGet("{id}/products/export")]
        public async Task<IActionResult> ExportProposalProducts(int id)
        {
            try
            {
                var fileBytes = await _excelService.ExportProposalProductsAsync(id);
                var fileName = $"Proposal_{id}_Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting proposal products for proposal {ProposalId}", id);
                return StatusCode(500, new { message = "Error exporting proposal products" });
            }
        }

        /// <summary>
        /// Download Excel template with all products for a manufacturer
        /// </summary>
        [HttpGet("products/excel-template/{manufacturerId}")]
        public async Task<IActionResult> DownloadProductsTemplate(int manufacturerId)
        {
            try
            {
                var fileBytes = await _excelService.GenerateTemplateAsync(manufacturerId);
                var fileName = $"Proposal_Products_Template_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel template for manufacturer {ManufacturerId}", manufacturerId);
                return StatusCode(500, new { message = "Error generating Excel template", error = ex.Message });
            }
        }

        /// <summary>
        /// Upload and parse Excel file with product pricing
        /// </summary>
        [HttpPost("products/excel-import/{manufacturerId}")]
        public async Task<ActionResult<ProposalProductExcelImportResponse>> UploadProductsExcel(int manufacturerId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return BadRequest(new { message = "Invalid file type. Only Excel files (.xlsx, .xls) are supported." });
                }

                if (file.Length > 10 * 1024 * 1024) // 10MB limit
                {
                    return BadRequest(new { message = "File size exceeds 10MB limit" });
                }

                using var stream = file.OpenReadStream();
                var result = await _excelService.ImportFromExcelAsync(stream, manufacturerId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing Excel file for manufacturer {ManufacturerId}", manufacturerId);
                return StatusCode(500, new { message = "Error importing Excel file", error = ex.Message });
            }
        }

    }
}

