using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractManufacturerVersionRepository
    {
        Task<IEnumerable<ContractManufacturerVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? manufacturerId = null);
        Task<ContractManufacturerVersion?> GetByIdAsync(int id);
        Task<ContractManufacturerVersion> CreateAsync(ContractManufacturerVersion entity);
        Task<ContractManufacturerVersion> UpdateAsync(ContractManufacturerVersion entity);
        Task<bool> DeleteAsync(int id);
    }
}

