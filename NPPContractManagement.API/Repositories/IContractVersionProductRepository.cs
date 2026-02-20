using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractVersionProductRepository
    {
        Task<IEnumerable<ContractVersionProduct>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? productId = null);
        Task<ContractVersionProduct?> GetByIdAsync(int id);
        Task<ContractVersionProduct> CreateAsync(ContractVersionProduct entity);
        Task<ContractVersionProduct> UpdateAsync(ContractVersionProduct entity);
        Task<bool> DeleteAsync(int id);
    }
}

