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
    public class DistributorsController : ControllerBase
    {
        private readonly IDistributorService _distributorService;
        private readonly ILogger<DistributorsController> _logger;

        public DistributorsController(IDistributorService distributorService, ILogger<DistributorsController> logger)
        {
            _distributorService = distributorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<DistributorsDto>>> GetAllDistributors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] bool? receiveContractProposal = null,
            [FromQuery] string? state = null)
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

                var (distributors, totalCount) = await _distributorService.SearchDistributorsAsync(
                    searchTerm ?? string.Empty,
                    statusInt,
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    receiveContractProposal,
                    state);

                // Map page items to DTOs
                var items = distributors.Select(d => new DistributorsDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    ContactPerson = d.ContactPerson,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Address = d.Address,
                    City = d.City,
                    State = d.State,
                    ZipCode = d.ZipCode,
                    Country = d.Country,
                    ReceiveContractProposal = d.ReceiveContractProposal,
                    Status = d.Status,
                    StatusName = d.StatusName,
                    IsActive = d.IsActive,
                    CreatedDate = d.CreatedDate,
                    ModifiedDate = d.ModifiedDate,
                    CreatedBy = d.CreatedBy,
                    ModifiedBy = d.ModifiedBy,
                    OpCosCount = d.OpCosCount,
                    CustomerAccountsCount = d.CustomerAccountsCount
                }).ToList();

                var response = new PaginatedResult<DistributorsDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all distributors");
                return StatusCode(500, new { message = "An error occurred while retrieving distributors" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DistributorDto>> GetDistributor(int id)
        {
            try
            {
                var distributor = await _distributorService.GetDistributorByIdAsync(id);
                if (distributor == null)
                {
                    return NotFound(new { message = "Distributor not found" });
                }

                return Ok(distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distributor {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the distributor" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<DistributorDto>> CreateDistributor([FromBody] CreateDistributorDto createDistributorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var distributor = await _distributorService.CreateDistributorAsync(createDistributorDto, currentUser);

                return CreatedAtAction(nameof(GetDistributor), new { id = distributor.Id }, distributor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating distributor");
                return StatusCode(500, new { message = "An error occurred while creating the distributor" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<DistributorDto>> UpdateDistributor(int id, [FromBody] UpdateDistributorDto updateDistributorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var distributor = await _distributorService.UpdateDistributorAsync(id, updateDistributorDto, currentUser);

                return Ok(distributor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating distributor {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the distributor" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult> DeleteDistributor(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _distributorService.DeactivateDistributorAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Distributor not found" });
                }

                return Ok(new { message = "Distributor marked inactive (soft-deleted)" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting distributor {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the distributor" });
            }
        }

        [HttpPost("{id}/activate")]
        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> ActivateDistributor(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _distributorService.ActivateDistributorAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Distributor not found" });
                }

                return Ok(new { message = "Distributor activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating distributor {Id}", id);
                return StatusCode(500, new { message = "An error occurred while activating the distributor" });
            }
        }

        [HttpPost("{id}/deactivate")]
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> DeactivateDistributor(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _distributorService.DeactivateDistributorAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Distributor not found" });
                }

                return Ok(new { message = "Distributor deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating distributor {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deactivating the distributor" });
            }
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }
    }
}
