using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Repositories;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Industries
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IndustriesController : ControllerBase
    {
        private readonly IIndustryService _industryService;

        public IndustriesController(IIndustryService industryService)
        {
            _industryService = industryService;
        }

        /// <summary>
        /// Get paginated Industries with search, sorting and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <param name="sortBy">Sort field name</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="status">Filter by status</param>
        /// <returns>Paginated list of Industries</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<IndustryDto>>> GetIndustries(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                // Determine status filter (int?) from the status query param
                int? statusInt = null;
                var statusText = status ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(statusText))
                {
                    if (statusText.Equals("Active", StringComparison.OrdinalIgnoreCase)) statusInt = 1;
                    else if (statusText.Equals("Inactive", StringComparison.OrdinalIgnoreCase)) statusInt = 2;
                    else if (statusText.Equals("Pending", StringComparison.OrdinalIgnoreCase)) statusInt = 3;
                }

                // Use SearchIndustriesAsync with status filter
                var (industries, totalCount) = await _industryService.SearchIndustriesAsync(
                    searchTerm ?? string.Empty,
                    statusInt,
                    pageNumber,
                    pageSize);

                // Only include active industries
                var activeIndustries = industries.Where(i => i.IsActive).ToList();
                var activeTotal = activeIndustries.Count;

                var response = new PaginatedResult<IndustryDto>
                {
                    Items = activeIndustries,
                    TotalCount = activeTotal,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)activeTotal / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Industries", error = ex.Message });
            }
        }

        /// <summary>
        /// Get paginated Industries with search and filtering
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 12)</param>
        /// <param name="search">Search term for name</param>
        /// <param name="status">Filter by status</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <returns>Paginated list of Industries</returns>
        [HttpGet("paginated")]
        public async Task<ActionResult<object>> GetPaginatedIndustries(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = "asc")
        {
            try
            {
                // Convert status string to int if provided
                int? statusInt = null;
                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        statusInt = 1;
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        statusInt = 2;
                    else if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                        statusInt = 3;
                }

                var (industries, totalCount) = await _industryService.SearchIndustriesAsync(
                    search ?? string.Empty,
                    statusInt,
                    page,
                    pageSize);

                var response = new
                {
                    items = industries,
                    totalCount = totalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving paginated Industries", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Industry by ID
        /// </summary>
        /// <param name="id">Industry ID</param>
        /// <returns>Industry details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IndustryDto>> GetIndustryById(int id)
        {
            try
            {
                var industry = await _industryService.GetIndustryByIdAsync(id);
                if (industry == null)
                {
                    return NotFound(new { message = "Industry not found" });
                }
                return Ok(industry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Industry
        /// </summary>
        /// <param name="createIndustryDto">Industry creation data</param>
        /// <returns>Created Industry</returns>
        [HttpPost]
        public async Task<ActionResult<IndustryDto>> CreateIndustry([FromBody] CreateIndustryDto createIndustryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var industry = await _industryService.CreateIndustryAsync(createIndustryDto, createdBy);
                return CreatedAtAction(nameof(GetIndustryById), new { id = industry.Id }, industry);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing Industry
        /// </summary>
        /// <param name="id">Industry ID</param>
        /// <param name="updateIndustryDto">Industry update data</param>
        /// <returns>Updated Industry</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IndustryDto>> UpdateIndustry(int id, [FromBody] UpdateIndustryDto updateIndustryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var industry = await _industryService.UpdateIndustryAsync(id, updateIndustryDto, modifiedBy);
                return Ok(industry);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete an Industry
        /// </summary>
        /// <param name="id">Industry ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIndustry(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _industryService.DeactivateIndustryAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Industry not found" });
                }
                return Ok(new { message = "Industry marked inactive (soft-deleted)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate an Industry
        /// </summary>
        /// <param name="id">Industry ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult> ActivateIndustry(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _industryService.ActivateIndustryAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Industry not found" });
                }
                return Ok(new { message = "Industry activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate an Industry
        /// </summary>
        /// <param name="id">Industry ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateIndustry(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _industryService.DeactivateIndustryAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Industry not found" });
                }
                return Ok(new { message = "Industry deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Industry by name
        /// </summary>
        /// <param name="name">Industry name</param>
        /// <returns>Industry details</returns>
        [HttpGet("name/{name}")]
        public async Task<ActionResult<IndustryDto>> GetIndustryByName(string name)
        {
            try
            {
                var industry = await _industryService.GetIndustryByNameAsync(name);
                if (industry == null)
                {
                    return NotFound(new { message = "Industry not found" });
                }
                return Ok(industry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Industry", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Industries by status
        /// </summary>
        /// <param name="status">Status (1=Active, 2=Inactive, 3=Pending)</param>
        /// <returns>List of Industries with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<IndustryDto>>> GetIndustriesByStatus(int status)
        {
            try
            {
                var industries = await _industryService.GetIndustriesByStatusAsync(status);
                return Ok(industries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Industries", error = ex.Message });
            }
        }

        /// <summary>
        /// Get active Industries
        /// </summary>
        /// <returns>List of active Industries</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<IndustryDto>>> GetActiveIndustries()
        {
            try
            {
                var industries = await _industryService.GetActiveIndustriesAsync();
                return Ok(industries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active Industries", error = ex.Message });
            }
        }

        /// <summary>
        /// Search Industries with pagination and filtering
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Industries</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchIndustries(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (industries, totalCount) = await _industryService.SearchIndustriesAsync(searchTerm, status, page, pageSize);
                
                return Ok(new
                {
                    data = industries,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Industries", error = ex.Message });
            }
        }
    }
}
