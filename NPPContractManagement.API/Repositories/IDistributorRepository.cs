using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IDistributorRepository : IRepository<Distributor>
    {
        Task<Distributor?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<IEnumerable<Distributor>> GetByStatusAsync(DistributorStatus status);
        Task<IEnumerable<Distributor>> GetByReceiveContractProposalAsync(bool receiveContractProposal);
        Task<IEnumerable<Distributor>> SearchAsync(string searchTerm, DistributorStatus? status = null, bool? receiveContractProposal = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? state = null);
        Task<int> GetCountAsync(string searchTerm, DistributorStatus? status = null, bool? receiveContractProposal = null, string? state = null);
        Task<IEnumerable<Distributor>> GetActiveDistributorsAsync();
    }
}
