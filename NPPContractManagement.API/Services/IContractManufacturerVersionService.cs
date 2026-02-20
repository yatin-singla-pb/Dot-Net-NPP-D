using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractManufacturerVersionService
    {
        Task<IEnumerable<ContractManufacturerVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? manufacturerId = null);
        Task<ContractManufacturerVersionDto?> GetByIdAsync(int id);
        Task<ContractManufacturerVersionDto> CreateAsync(int contractId, int manufacturerId, int versionNumber, string? assignedBy, DateTime? assignedDate);
        Task<ContractManufacturerVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate);
        Task<bool> DeleteAsync(int id);
    }
}

