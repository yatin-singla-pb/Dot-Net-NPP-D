using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IContractVersionRepository
    {
        Task<IEnumerable<ContractVersion>> GetVersionsAsync(int contractId);
        Task<ContractVersion?> GetVersionAsync(int contractId, int versionId);
        Task<ContractVersion> CreateVersionAsync(int contractId, ContractVersion version, IEnumerable<ContractVersionPrice> prices, string createdBy);
        Task<ContractVersion> UpdateVersionAsync(ContractVersion version, string modifiedBy);
    }
}

