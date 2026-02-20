using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractPriceService
    {
        Task<IEnumerable<ContractPriceDto>> GetAllAsync(int? productId = null, int? versionNumber = null);
        Task<ContractPriceDto?> GetByIdAsync(int id);
        Task<ContractPriceDto> CreateAsync(CreateContractPriceRequest request, string createdBy);
        Task<ContractPriceDto> UpdateAsync(int id, UpdateContractPriceRequest request, string modifiedBy);
        Task<bool> DeleteAsync(int id);
    }
}

