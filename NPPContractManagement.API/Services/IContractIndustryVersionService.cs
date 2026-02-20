using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractIndustryVersionService
    {
        Task<IEnumerable<ContractIndustryVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? industryId = null);
        Task<ContractIndustryVersionDto?> GetByIdAsync(int id);
        Task<ContractIndustryVersionDto> CreateAsync(CreateContractIndustryVersionRequest request);
        Task<ContractIndustryVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate);
        Task<bool> DeleteAsync(int id);
    }
}

