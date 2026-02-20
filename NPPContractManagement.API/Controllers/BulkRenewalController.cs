using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "System Administrator,Contract Manager")]
    public class BulkRenewalController : ControllerBase
    {
        private readonly IBulkRenewalService _bulkRenewalService;
        private readonly ILogger<BulkRenewalController> _logger;

        public BulkRenewalController(
            IBulkRenewalService bulkRenewalService,
            ILogger<BulkRenewalController> logger)
        {
            _bulkRenewalService = bulkRenewalService;
            _logger = logger;
        }

        /// <summary>
        /// Create multiple proposal requests from selected contracts
        /// </summary>
        /// <param name="request">Bulk renewal request with contract IDs and pricing adjustments</param>
        /// <returns>Bulk renewal response with results for each contract</returns>
        [HttpPost("create")]
        public async Task<ActionResult<BulkRenewalResponse>> CreateBulkRenewal([FromBody] BulkRenewalRequest request)
        {
            try
            {
                if (request.ContractIds == null || !request.ContractIds.Any())
                {
                    return BadRequest(new { message = "At least one contract must be selected" });
                }

                // Set created by from current user
                request.CreatedBy = User.Identity?.Name ?? "Unknown";

                _logger.LogInformation("User {User} initiating bulk renewal for {Count} contracts",
                    request.CreatedBy, request.ContractIds.Count);

                var response = await _bulkRenewalService.CreateBulkRenewalProposalsAsync(request);

                // Return 200 even with partial failures — client handles individual errors
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk renewal request");
                return StatusCode(500, new
                {
                    message = "An error occurred while processing the bulk renewal request",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Validate contracts can be renewed
        /// </summary>
        /// <param name="contractIds">List of contract IDs to validate</param>
        /// <returns>Validation results for each contract</returns>
        [HttpPost("validate")]
        public async Task<ActionResult<Dictionary<int, string>>> ValidateContracts([FromBody] List<int> contractIds)
        {
            try
            {
                if (contractIds == null || !contractIds.Any())
                {
                    return BadRequest(new { message = "At least one contract ID must be provided" });
                }

                var validationResults = await _bulkRenewalService.ValidateContractsForRenewalAsync(contractIds);
                return Ok(validationResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating contracts for renewal");
                return StatusCode(500, new
                {
                    message = "An error occurred while validating contracts",
                    error = ex.Message
                });
            }
        }

    }
}

