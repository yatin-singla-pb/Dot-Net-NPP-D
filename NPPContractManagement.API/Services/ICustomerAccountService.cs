using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface ICustomerAccountService
    {
        Task<IEnumerable<CustomerAccountDto>> GetAllCustomerAccountsAsync();
        Task<CustomerAccountDto?> GetCustomerAccountByIdAsync(int id);
        Task<CustomerAccountDto> CreateCustomerAccountAsync(CreateCustomerAccountDto createCustomerAccountDto, string createdBy);
        Task<CustomerAccountDto> UpdateCustomerAccountAsync(int id, UpdateCustomerAccountDto updateCustomerAccountDto, string modifiedBy);
        Task<bool> DeleteCustomerAccountAsync(int id);
        Task<bool> ActivateCustomerAccountAsync(int id, string modifiedBy);
        Task<bool> DeactivateCustomerAccountAsync(int id, string modifiedBy);
        Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByMemberAccountIdAsync(int memberAccountId);
        Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByDistributorIdAsync(int distributorId);
        Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByOpCoIdAsync(int opCoId);
        Task<IEnumerable<CustomerAccountDto>> GetCustomerAccountsByStatusAsync(int status);
        Task<CustomerAccountDto?> GetCustomerAccountByAccountNumberAsync(int distributorId, string customerAccountNumber);
        Task<(IEnumerable<CustomerAccountDto> CustomerAccounts, int TotalCount)> SearchCustomerAccountsAsync(string searchTerm, int? memberAccountId = null, int? distributorId = null, int? opCoId = null, int? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null, int page = 1, int pageSize = 10);
    }
}
