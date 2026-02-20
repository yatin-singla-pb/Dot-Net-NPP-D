using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractOpCoVersionService
    {
        Task<IEnumerable<ContractOpCoVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? opCoId = null);
        Task<ContractOpCoVersionDto?> GetByIdAsync(int id);
        Task<ContractOpCoVersionDto> CreateAsync(int contractId, int opCoId, int versionNumber, string? assignedBy, DateTime? assignedDate);
        Task<ContractOpCoVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate);
        Task<bool> DeleteAsync(int id);
    }
}

