using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Repositories;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Op-Cos (Operating Companies)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OpCosController : ControllerBase
    {
        private readonly IOpCoService _opCoService;

        public OpCosController(IOpCoService opCoService)
        {
            _opCoService = opCoService;
        }

        /// <summary>
        /// Get paginated Op-Cos with search, sorting, and filtering (defaults to active only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<OpCoDto>>> GetAllOpCos(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] string? remoteReferenceCode = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                int? statusInt = null;
                var statusText = status ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(statusText))
                {
                    if (statusText.Equals("Active", StringComparison.OrdinalIgnoreCase)) statusInt = 1;
                    else if (statusText.Equals("Inactive", StringComparison.OrdinalIgnoreCase)) statusInt = 2;
                    else if (statusText.Equals("Pending", StringComparison.OrdinalIgnoreCase)) statusInt = 3;
                }

                var (opCos, totalCount) = await _opCoService.SearchOpCosAsync(
                    searchTerm ?? string.Empty,
                    distributorId,
                    statusInt,
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    remoteReferenceCode);

                // Always return active Op-Cos only
                var items = opCos.Where(o => o.IsActive).ToList();
                var count = items.Count;

                var response = new PaginatedResult<OpCoDto>
                {
                    Items = items,
                    TotalCount = count,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)count / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Op-Cos", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Op-Co by ID
        /// </summary>
        /// <param name="id">Op-Co ID</param>
        /// <returns>Op-Co details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OpCoDto>> GetOpCoById(int id)
        {
            try
            {
                var opCo = await _opCoService.GetOpCoByIdAsync(id);
                if (opCo == null)
                {
                    return NotFound(new { message = "Op-Co not found" });
                }
                return Ok(opCo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Op-Co
        /// </summary>
        /// <param name="createOpCoDto">Op-Co creation data</param>
        /// <returns>Created Op-Co</returns>
        [HttpPost]
        public async Task<ActionResult<OpCoDto>> CreateOpCo([FromBody] CreateOpCoDto createOpCoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var opCo = await _opCoService.CreateOpCoAsync(createOpCoDto, createdBy);
                return CreatedAtAction(nameof(GetOpCoById), new { id = opCo.Id }, opCo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing Op-Co
        /// </summary>
        /// <param name="id">Op-Co ID</param>
        /// <param name="updateOpCoDto">Op-Co update data</param>
        /// <returns>Updated Op-Co</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<OpCoDto>> UpdateOpCo(int id, [FromBody] UpdateOpCoDto updateOpCoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var opCo = await _opCoService.UpdateOpCoAsync(id, updateOpCoDto, modifiedBy);
                return Ok(opCo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete (soft-delete) an Op-Co by marking it inactive
        /// </summary>
        /// <param name="id">Op-Co ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOpCo(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _opCoService.DeactivateOpCoAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Op-Co not found" });
                }
                return Ok(new { message = "Op-Co marked inactive (soft-deleted)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate an Op-Co
        /// </summary>
        /// <param name="id">Op-Co ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult> ActivateOpCo(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _opCoService.ActivateOpCoAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Op-Co not found" });
                }
                return Ok(new { message = "Op-Co activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate an Op-Co
        /// </summary>
        /// <param name="id">Op-Co ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateOpCo(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _opCoService.DeactivateOpCoAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Op-Co not found" });
                }
                return Ok(new { message = "Op-Co deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Op-Cos by distributor ID
        /// </summary>
        /// <param name="distributorId">Distributor ID</param>
        /// <returns>List of Op-Cos for the distributor</returns>
        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<OpCoDto>>> GetOpCosByDistributorId(int distributorId)
        {
            try
            {
                var opCos = await _opCoService.GetOpCosByDistributorIdAsync(distributorId);
                return Ok(opCos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Op-Cos", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Op-Cos by multiple distributor IDs (deduplicated)
        /// </summary>
        [HttpPost("by-distributors")]
        public async Task<ActionResult<IEnumerable<OpCoDto>>> GetOpCosByDistributors([FromBody] int[] distributorIds)
        {
            try
            {
                var ids = distributorIds ?? Array.Empty<int>();
                var opCos = await _opCoService.GetOpCosByDistributorIdsAsync(ids);
                // Default to active only like UI expectation
                var items = opCos.Where(o => o.IsActive).GroupBy(o => o.Id).Select(g => g.First()).OrderBy(o => o.Name).ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Op-Cos", error = ex.Message });
            }
        }


        /// <summary>
        /// Get Op-Cos by status
        /// </summary>
        /// <param name="status">Status (1=Active, 2=Inactive, 3=Pending)</param>
        /// <returns>List of Op-Cos with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OpCoDto>>> GetOpCosByStatus(int status)
        {
            try
            {
                var opCos = await _opCoService.GetOpCosByStatusAsync(status);
                return Ok(opCos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Op-Cos", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Op-Co by remote reference code
        /// </summary>
        /// <param name="remoteReferenceCode">Remote reference code</param>
        /// <returns>Op-Co details</returns>
        [HttpGet("remote-reference/{remoteReferenceCode}")]
        public async Task<ActionResult<OpCoDto>> GetOpCoByRemoteReferenceCode(string remoteReferenceCode)
        {
            try
            {
                var opCo = await _opCoService.GetOpCoByRemoteReferenceCodeAsync(remoteReferenceCode);
                if (opCo == null)
                {
                    return NotFound(new { message = "Op-Co not found" });
                }
                return Ok(opCo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Op-Co", error = ex.Message });
            }
        }

        /// <summary>
        /// Search Op-Cos with pagination, sorting, and filtering
        /// </summary>
        /// <param name="search">Search term</param>
        /// <param name="distributorId">Filter by distributor ID</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <param name="remoteReferenceCode">Filter by remote reference code</param>
        /// <returns>Paginated list of Op-Cos</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchOpCos(
            [FromQuery] string? search = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? remoteReferenceCode = null)
        {
            try
            {
                var (opCos, totalCount) = await _opCoService.SearchOpCosAsync(search, distributorId, status, page, pageSize, sortBy, sortDirection, remoteReferenceCode);

                return Ok(new
                {
                    data = opCos,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Op-Cos", error = ex.Message });
            }
        }

        /// <summary>
        /// Get paginated Op-Cos (standard endpoint used by UI)
        /// </summary>
        [HttpGet("paginated")]
        public async Task<ActionResult<object>> GetPaginatedOpCos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? remoteReferenceCode = null)
        {
            try
            {
                int? statusInt = null;
                if (!string.IsNullOrWhiteSpace(status))
                {
                    statusInt = status?.ToLower() switch
                    {
                        "active" => 1,
                        "inactive" => 2,
                        "pending" => 3,
                        _ => null
                    };
                }

                var (opCos, totalCount) = await _opCoService.SearchOpCosAsync(search, distributorId, statusInt, page, pageSize, sortBy, sortDirection, remoteReferenceCode);

                // Default to active only if no status supplied
                var items = string.IsNullOrWhiteSpace(status)
                    ? opCos.Where(o => o.IsActive).ToList()
                    : opCos.ToList();
                var count = string.IsNullOrWhiteSpace(status) ? items.Count : totalCount;

                return Ok(new
                {
                    items = items,
                    totalCount = count,
                    pageNumber = page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)count / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Op-Cos", error = ex.Message });
            }
        }
    }
}
