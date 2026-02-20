using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Repositories;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Customer Accounts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerAccountsController : ControllerBase
    {
        private readonly ICustomerAccountService _customerAccountService;

        public CustomerAccountsController(ICustomerAccountService customerAccountService)
        {
            _customerAccountService = customerAccountService;
        }

        /// <summary>
        /// Get all Customer Accounts
        /// </summary>
        /// <returns>List of Customer Accounts</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<CustomerAccountDto>>> GetAllCustomerAccounts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "CustomerName",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                int? statusInt = null;
                var statusText = status ?? string.Empty;
                if (string.IsNullOrWhiteSpace(statusText))
                {
                    // No status filter by default; rely on IsActive to control listing
                    statusInt = null;
                }
                else
                {
                    if (statusText.Equals("Active", StringComparison.OrdinalIgnoreCase)) statusInt = 1;
                    else if (statusText.Equals("Inactive", StringComparison.OrdinalIgnoreCase)) statusInt = 2;
                    else if (statusText.Equals("Pending", StringComparison.OrdinalIgnoreCase)) statusInt = 3;
                    else if (statusText.Equals("Suspended", StringComparison.OrdinalIgnoreCase)) statusInt = 4;
                    else if (statusText.Equals("Closed", StringComparison.OrdinalIgnoreCase)) statusInt = 5;
                }

                var (customerAccounts, totalCount) = await _customerAccountService.SearchCustomerAccountsAsync(
                    searchTerm ?? string.Empty,
                    null,
                    null,
                    null,
                    statusInt,
                    true, // isActive only
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null, // state
                    pageNumber,
                    pageSize);

                var response = new PaginatedResult<CustomerAccountDto>
                {
                    Items = customerAccounts,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Customer Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Account by ID
        /// </summary>
        /// <param name="id">Customer Account ID</param>
        /// <returns>Customer Account details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerAccountDto>> GetCustomerAccountById(int id)
        {
            try
            {
                var customerAccount = await _customerAccountService.GetCustomerAccountByIdAsync(id);
                if (customerAccount == null)
                {
                    return NotFound(new { message = "Customer Account not found" });
                }
                return Ok(customerAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new Customer Account
        /// </summary>
        /// <param name="createCustomerAccountDto">Customer Account creation data</param>
        /// <returns>Created Customer Account</returns>
        [HttpPost]
        public async Task<ActionResult<CustomerAccountDto>> CreateCustomerAccount([FromBody] CreateCustomerAccountDto createCustomerAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var customerAccount = await _customerAccountService.CreateCustomerAccountAsync(createCustomerAccountDto, createdBy);
                return CreatedAtAction(nameof(GetCustomerAccountById), new { id = customerAccount.Id }, customerAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing Customer Account
        /// </summary>
        /// <param name="id">Customer Account ID</param>
        /// <param name="updateCustomerAccountDto">Customer Account update data</param>
        /// <returns>Updated Customer Account</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerAccountDto>> UpdateCustomerAccount(int id, [FromBody] UpdateCustomerAccountDto updateCustomerAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var customerAccount = await _customerAccountService.UpdateCustomerAccountAsync(id, updateCustomerAccountDto, modifiedBy);
                return Ok(customerAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a Customer Account
        /// </summary>
        /// <param name="id">Customer Account ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomerAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _customerAccountService.DeactivateCustomerAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Customer Account not found" });
                }
                return Ok(new { message = "Customer Account marked inactive (soft-deleted)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate a Customer Account
        /// </summary>
        /// <param name="id">Customer Account ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult> ActivateCustomerAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _customerAccountService.ActivateCustomerAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Customer Account not found" });
                }
                return Ok(new { message = "Customer Account activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a Customer Account
        /// </summary>
        /// <param name="id">Customer Account ID</param>
        /// <returns>Success status</returns>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> DeactivateCustomerAccount(int id)
        {
            try
            {
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var result = await _customerAccountService.DeactivateCustomerAccountAsync(id, modifiedBy);
                if (!result)
                {
                    return NotFound(new { message = "Customer Account not found" });
                }
                return Ok(new { message = "Customer Account deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating the Customer Account", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Accounts by member account ID
        /// </summary>
        /// <param name="memberAccountId">Member Account ID</param>
        /// <returns>List of Customer Accounts for the member account</returns>
        [HttpGet("member-account/{memberAccountId}")]
        public async Task<ActionResult<IEnumerable<CustomerAccountDto>>> GetCustomerAccountsByMemberAccountId(int memberAccountId)
        {
            try
            {
                var customerAccounts = await _customerAccountService.GetCustomerAccountsByMemberAccountIdAsync(memberAccountId);
                return Ok(customerAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Customer Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Accounts by distributor ID
        /// </summary>
        /// <param name="distributorId">Distributor ID</param>
        /// <returns>List of Customer Accounts for the distributor</returns>
        [HttpGet("distributor/{distributorId}")]
        public async Task<ActionResult<IEnumerable<CustomerAccountDto>>> GetCustomerAccountsByDistributorId(int distributorId)
        {
            try
            {
                var customerAccounts = await _customerAccountService.GetCustomerAccountsByDistributorIdAsync(distributorId);
                return Ok(customerAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Customer Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Accounts by OpCo ID
        /// </summary>
        /// <param name="opCoId">OpCo ID</param>
        /// <returns>List of Customer Accounts for the OpCo</returns>
        [HttpGet("opco/{opCoId}")]
        public async Task<ActionResult<IEnumerable<CustomerAccountDto>>> GetCustomerAccountsByOpCoId(int opCoId)
        {
            try
            {
                var customerAccounts = await _customerAccountService.GetCustomerAccountsByOpCoIdAsync(opCoId);
                return Ok(customerAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Customer Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Accounts by status
        /// </summary>
        /// <param name="status">Status (1=Active, 2=Inactive, 3=Pending)</param>
        /// <returns>List of Customer Accounts with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<CustomerAccountDto>>> GetCustomerAccountsByStatus(int status)
        {
            try
            {
                var customerAccounts = await _customerAccountService.GetCustomerAccountsByStatusAsync(status);
                return Ok(customerAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Customer Accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Get Customer Account by account number and distributor
        /// </summary>
        /// <param name="distributorId">Distributor ID</param>
        /// <param name="customerAccountNumber">Customer Account Number</param>
        /// <returns>Customer Account details</returns>
        [HttpGet("distributor/{distributorId}/account-number/{customerAccountNumber}")]
        public async Task<ActionResult<CustomerAccountDto>> GetCustomerAccountByAccountNumber(int distributorId, string customerAccountNumber)
        {
            try
            {
                var customerAccount = await _customerAccountService.GetCustomerAccountByAccountNumberAsync(distributorId, customerAccountNumber);
                if (customerAccount == null)
                {
                    return NotFound(new { message = "Customer Account not found" });
                }
                return Ok(customerAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Customer Account", error = ex.Message });
            }
        }



        /// <summary>
        /// Search Customer Accounts with pagination and filtering
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="memberAccountId">Filter by member account ID</param>
        /// <param name="distributorId">Filter by distributor ID</param>
        /// <param name="opCoId">Filter by OpCo ID</param>
        /// <param name="status">Filter by status</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="industryId">Filter by industry ID</param>
        /// <param name="association">Filter by association type</param>
        /// <param name="startDate">Filter by start date</param>
        /// <param name="endDate">Filter by end date</param>
        /// <param name="tracsAccess">Filter by TRACS access</param>
        /// <param name="toEntegra">Filter by Entegra flag</param>
        /// <param name="state">Filter by state</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Customer Accounts</returns>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchCustomerAccounts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? memberAccountId = null,
            [FromQuery] int? distributorId = null,
            [FromQuery] int? opCoId = null,
            [FromQuery] int? status = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? industryId = null,
            [FromQuery] int? association = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool? tracsAccess = null,
            [FromQuery] bool? toEntegra = null,
            [FromQuery] string? state = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (customerAccounts, totalCount) = await _customerAccountService.SearchCustomerAccountsAsync(
                    searchTerm, memberAccountId, distributorId, opCoId, status, isActive, industryId, association, startDate, endDate, tracsAccess, toEntegra, state, page, pageSize);

                return Ok(new
                {
                    data = customerAccounts,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching Customer Accounts", error = ex.Message });
            }
        }
    }
}
