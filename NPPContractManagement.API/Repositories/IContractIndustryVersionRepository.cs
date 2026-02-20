using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractIndustryVersionRepository
    {
        Task<IEnumerable<ContractIndustryVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? industryId = null);
        Task<ContractIndustryVersion?> GetByIdAsync(int id);
        Task<ContractIndustryVersion> CreateAsync(ContractIndustryVersion entity);
        Task<ContractIndustryVersion> UpdateAsync(ContractIndustryVersion entity);
        Task<bool> DeleteAsync(int id);
    }
}

