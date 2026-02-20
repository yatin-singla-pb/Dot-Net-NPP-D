using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractDistributorVersionRepository
    {
        Task<IEnumerable<ContractDistributorVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? distributorId = null);
        Task<ContractDistributorVersion?> GetByIdAsync(int id);
        Task<ContractDistributorVersion> CreateAsync(ContractDistributorVersion entity);
        Task<ContractDistributorVersion> UpdateAsync(ContractDistributorVersion entity);
        Task<bool> DeleteAsync(int id);
    }
}

