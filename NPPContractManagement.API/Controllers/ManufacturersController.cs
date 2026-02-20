using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Repositories;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly ILogger<ManufacturersController> _logger;

        public ManufacturersController(IManufacturerService manufacturerService, ILogger<ManufacturersController> logger)
        {
            _manufacturerService = manufacturerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ManufacturerDto>>> GetAllManufacturers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] string? state = null,
            [FromQuery] int? primaryBrokerId = null)
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
                }

                var (manufacturers, totalCount) = await _manufacturerService.SearchManufacturersAsync(
                    searchTerm ?? string.Empty,
                    statusInt,
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    state,
                    primaryBrokerId);

                var response = new PaginatedResult<ManufacturerDto>
                {
                    Items = manufacturers.ToList(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all manufacturers");
                return StatusCode(500, new { message = "An error occurred while retrieving manufacturers" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerDto>> GetManufacturer(int id)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                if (manufacturer == null)
                {
                    return NotFound(new { message = "Manufacturer not found" });
                }

                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting manufacturer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the manufacturer" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<ManufacturerDto>> CreateManufacturer([FromBody] CreateManufacturerDto createManufacturerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var manufacturer = await _manufacturerService.CreateManufacturerAsync(createManufacturerDto, currentUser);

                return CreatedAtAction(nameof(GetManufacturer), new { id = manufacturer.Id }, manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating manufacturer");
                return StatusCode(500, new { message = "An error occurred while creating the manufacturer" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<ManufacturerDto>> UpdateManufacturer(int id, [FromBody] UpdateManufacturerDto updateManufacturerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var manufacturer = await _manufacturerService.UpdateManufacturerAsync(id, updateManufacturerDto, currentUser);

                return Ok(manufacturer);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating manufacturer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the manufacturer" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult> DeleteManufacturer(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _manufacturerService.DeactivateManufacturerAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Manufacturer not found" });
                }

                return Ok(new { message = "Manufacturer marked inactive (soft-deleted)" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting manufacturer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the manufacturer" });
            }
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> ActivateManufacturer(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _manufacturerService.ActivateManufacturerAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Manufacturer not found" });
                }

                return Ok(new { message = "Manufacturer activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating manufacturer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while activating the manufacturer" });
            }
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> DeactivateManufacturer(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _manufacturerService.DeactivateManufacturerAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Manufacturer not found" });
                }

                return Ok(new { message = "Manufacturer deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating manufacturer {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deactivating the manufacturer" });
            }
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }
    }
}
