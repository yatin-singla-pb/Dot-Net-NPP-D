using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface ICustomerAccountRepository : IRepository<CustomerAccount>
    {
        Task<IEnumerable<CustomerAccount>> GetByMemberAccountIdAsync(int memberAccountId);
        Task<IEnumerable<CustomerAccount>> GetByDistributorIdAsync(int distributorId);
        Task<IEnumerable<CustomerAccount>> GetByOpCoIdAsync(int opCoId);
        Task<IEnumerable<CustomerAccount>> GetByStatusAsync(CustomerAccountStatus status);
        Task<CustomerAccount?> GetByCustomerAccountNumberAsync(int distributorId, string customerAccountNumber);
        Task<bool> ExistsByCustomerAccountNumberAsync(int distributorId, string customerAccountNumber, int? excludeId = null);
        Task<IEnumerable<CustomerAccount>> SearchAsync(string searchTerm, int? memberAccountId = null, int? distributorId = null, int? opCoId = null, CustomerAccountStatus? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null, int page = 1, int pageSize = 10);
        Task<int> GetCountAsync(string searchTerm = "", int? memberAccountId = null, int? distributorId = null, int? opCoId = null, CustomerAccountStatus? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null);
    }
}
