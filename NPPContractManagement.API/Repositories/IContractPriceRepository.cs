using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractPriceRepository
    {
        Task<IEnumerable<ContractPrice>> GetAllAsync(int? productId = null, int? versionNumber = null);
        Task<ContractPrice?> GetByIdAsync(int id);
        Task<ContractPrice> CreateAsync(ContractPrice entity);
        Task<ContractPrice> UpdateAsync(ContractPrice entity);
        Task<bool> DeleteAsync(int id);
    }
}

