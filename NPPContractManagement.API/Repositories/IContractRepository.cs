using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetByManufacturerIdAsync(int manufacturerId);
        Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status);
        Task<IEnumerable<Contract>> GetByDistributorIdAsync(int distributorId);
        Task<IEnumerable<Contract>> GetByOpCoIdAsync(int opCoId);
        Task<IEnumerable<Contract>> GetByIndustryIdAsync(int industryId);
        Task<IEnumerable<Contract>> GetSuspendedContractsAsync();
        Task<IEnumerable<Contract>> GetContractsForPerformanceAsync();
        Task<IEnumerable<Contract>> GetExpiringContractsAsync(DateTime beforeDate);
        Task<IEnumerable<Contract>> GetExpiringContractsWithoutProposalsAsync(int daysThreshold);
        Task<IEnumerable<Contract>> SearchAsync(string searchTerm, int? manufacturerId = null, ContractStatus? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, IEnumerable<int>? allowedManufacturerIds = null);
        Task<int> GetCountAsync(int? manufacturerId = null, ContractStatus? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, IEnumerable<int>? allowedManufacturerIds = null);
        Task<bool> ValidateUniqueConstraintAsync(int manufacturerId, DateTime startDate, DateTime endDate, List<int> industryIds, List<int> opCoIds, int? excludeId = null);
    }
}
