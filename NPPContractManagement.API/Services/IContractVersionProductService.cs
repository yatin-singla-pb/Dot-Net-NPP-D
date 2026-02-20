using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractVersionProductService
    {
        Task<IEnumerable<ContractVersionProductDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? productId = null);
        Task<ContractVersionProductDto?> GetByIdAsync(int id);
        Task<ContractVersionProductDto> CreateAsync(int contractId, int productId, int versionNumber, string? assignedBy, DateTime? assignedDate);
        Task<ContractVersionProductDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate);
        Task<bool> DeleteAsync(int id);
    }
}

