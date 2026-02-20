using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IOpCoRepository : IRepository<OpCo>
    {
        Task<IEnumerable<OpCo>> GetByDistributorIdAsync(int distributorId);
        Task<IEnumerable<OpCo>> GetByStatusAsync(OpCoStatus status);
        Task<OpCo?> GetByRemoteReferenceCodeAsync(string remoteReferenceCode);
        Task<bool> ExistsByRemoteReferenceCodeAsync(string remoteReferenceCode, int? excludeId = null);
        Task<IEnumerable<OpCo>> SearchAsync(string? searchTerm, int? distributorId = null, OpCoStatus? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? remoteReferenceCode = null);
        Task<int> GetCountAsync(string? searchTerm = null, int? distributorId = null, OpCoStatus? status = null, string? remoteReferenceCode = null);
        Task<IEnumerable<OpCo>> GetByDistributorIdsAsync(IEnumerable<int> distributorIds);
    }
}