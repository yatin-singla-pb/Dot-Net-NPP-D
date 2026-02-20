using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Member Accounts
    /// </summary>
    [ApiController]
    [Route("api/member-accounts")]
    [Authorize]
    public class MemberAccountsController : ControllerBase
    {
        private readonly IMemberAccountService _memberAccountService;

        public MemberAccountsController(IMemberAccountService memberAccountService)
        {
            _memberAccountService = memberAccountService;
        }

        /// <summary>
        /// Get all Member Accounts
        /// </summary>
        /// <returns>List of Member Accounts</returns>
        [HttpGet]
        public async Task<ActionResult<NPPContractManagement.API.Repositories.PaginatedResult<MemberAccountDto>>> GetAllMemberAccounts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "FacilityName",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int? industryId = null,
            [FromQuery] string? w9 = null,
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
                    else if (statusText.Equals("Suspended", StringComparison.OrdinalIgnoreCase)) statusInt = 4;
                }

                var (memberAccounts, totalCount) = await _memberAccountService.SearchMemberAccountsAsync(
                    searchTerm ?? string.Empty,
                    industryId,
                    statusInt,
                    w9,
                    state,
                    pageNumber,
                    pageSize);

                var items = memberAccounts.ToList();
                var response = new NPPContractManagement.API.Repositories.PaginatedResult<MemberAccountDto>
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
                return StatusCode(500, new { message = "An error occurred while retrieving Member Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Member Account by ID
        /// </summary>
        /// <param name="id">Member Account ID</param>
        /// <returns>Member Account details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberAccountDto>> GetMemberAccountById(int id)
        {
            try
            {
                var memberAccount = await _memberAccountService.GetMemberAccountByIdAsync(id);
                if (memberAccount == null)
                {
                    return NotFound(new { message = "Member Account not found" });
                }
                return Ok(memberAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Member Account
        /// </summary>
        /// <param name="createMemberAccountDto">Member Account creation data</param>
        /// <returns>Created Member Account</returns>
        [HttpPost]
        public async Task<ActionResult<MemberAccountDto>> CreateMemberAccount([FromBody] CreateMemberAccountDto createMemberAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var memberAccount = await _memberAccountService.CreateMemberAccountAsync(createMemberAccountDto, createdBy);
                return CreatedAtAction(nameof(GetMemberAccountById), new { id = memberAccount.Id }, memberAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing Member Account
        /// </summary>
        /// <param name="id">Member Account ID</param>
        /// <param name="updateMemberAccountDto">Member Account update data</param>
        /// <returns>Updated Member Account</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<MemberAccountDto>> UpdateMemberAccount(int id, [FromBody] UpdateMemberAccountDto updateMemberAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var memberAccount = await _memberAccountService.UpdateMemberAccountAsync(id, updateMemberAccountDto, modifiedBy);
                return Ok(memberAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a Member Account
        /// </summary>
        /// <param name="id">Member Account ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMemberAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _memberAccountService.DeactivateMemberAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Member Account not found" });
                }
                return Ok(new { message = "Member Account marked inactive (soft-deleted)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a Member Account
        /// </summary>
        /// <param name="id">Member Account ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult> ActivateMemberAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _memberAccountService.ActivateMemberAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Member Account not found" });
                }
                return Ok(new { message = "Member Account activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a Member Account
        /// </summary>
        /// <param name="id">Member Account ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateMemberAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _memberAccountService.DeactivateMemberAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Member Account not found" });
                }
                return Ok(new { message = "Member Account deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Member Account by member number
        /// </summary>
        /// <param name="memberNumber">Member number</param>
        /// <returns>Member Account details</returns>
        [HttpGet("member-number/{memberNumber}")]
        public async Task<ActionResult<MemberAccountDto>> GetMemberAccountByMemberNumber(string memberNumber)
        {
            try
            {
                var memberAccount = await _memberAccountService.GetMemberAccountByMemberNumberAsync(memberNumber);
                if (memberAccount == null)
                {
                    return NotFound(new { message = "Member Account not found" });
                }
                return Ok(memberAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Member Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Member Accounts by industry ID
        /// </summary>
        /// <param name="industryId">Industry ID</param>
        /// <returns>List of Member Accounts for the industry</returns>
        [HttpGet("industry/{industryId}")]
        public async Task<ActionResult<IEnumerable<MemberAccountDto>>> GetMemberAccountsByIndustryId(int industryId)
        {
            try
            {
                var memberAccounts = await _memberAccountService.GetMemberAccountsByIndustryIdAsync(industryId);
                return Ok(memberAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Member Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Member Accounts by status
        /// </summary>
        /// <param name="status">Status (1=Active, 2=Inactive, 3=Pending)</param>
        /// <returns>List of Member Accounts with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<MemberAccountDto>>> GetMemberAccountsByStatus(int status)
        {
            try
            {
                var memberAccounts = await _memberAccountService.GetMemberAccountsByStatusAsync(status);
                return Ok(memberAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Member Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Search Member Accounts with pagination and filtering
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="industryId">Filter by industry ID</param>
        /// <param name="status">Filter by status</param>
        /// <param name="w9">Filter by W9 status</param>
        /// <param name="state">Filter by state</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Member Accounts</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchMemberAccounts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? industryId = null,
            [FromQuery] int? status = null,
            [FromQuery] string? w9 = null,
            [FromQuery] string? state = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (memberAccounts, totalCount) = await _memberAccountService.SearchMemberAccountsAsync(searchTerm, industryId, status, w9, state, page, pageSize);
                
                return Ok(new
                {
                    data = memberAccounts,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Member Accounts", error = ex.Message });
            }
        }
    }
}
