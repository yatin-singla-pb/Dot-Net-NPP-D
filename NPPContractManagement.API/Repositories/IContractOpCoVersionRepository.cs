using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractOpCoVersionRepository
    {
        Task<IEnumerable<ContractOpCoVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? opCoId = null);
        Task<ContractOpCoVersion?> GetByIdAsync(int id);
        Task<ContractOpCoVersion> CreateAsync(ContractOpCoVersion entity);
        Task<ContractOpCoVersion> UpdateAsync(ContractOpCoVersion entity);
        Task<bool> DeleteAsync(int id);
    }
}

