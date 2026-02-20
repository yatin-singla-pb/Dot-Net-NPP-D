using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractDistributorVersionService
    {
        Task<IEnumerable<ContractDistributorVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? distributorId = null);
        Task<ContractDistributorVersionDto?> GetByIdAsync(int id);
        Task<ContractDistributorVersionDto> CreateAsync(int contractId, int distributorId, int versionNumber, string? assignedBy, DateTime? assignedDate);
        Task<ContractDistributorVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate);
        Task<bool> DeleteAsync(int id);
    }
}

