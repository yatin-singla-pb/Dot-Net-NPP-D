using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IDistributorProductCodeRepository : IRepository<DistributorProductCode>
    {
        new Task<DistributorProductCode?> GetByIdAsync(int id);
        Task<IEnumerable<DistributorProductCode>> SearchAsync(
            string? searchTerm,
            IEnumerable<int>? distributorIds = null,
            IEnumerable<int>? productIds = null,
            int? productStatus = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc");
        Task<int> GetCountAsync(
            string? searchTerm,
            IEnumerable<int>? distributorIds = null,
            IEnumerable<int>? productIds = null,
            int? productStatus = null);
        Task<DistributorProductCode?> FindByDistributorAndCodeAsync(int distributorId, string distributorCode, int? excludeId = null);
        Task<DistributorProductCode?> FindByDistributorAndProductAsync(int distributorId, int productId, int? excludeId = null);
    }
}

