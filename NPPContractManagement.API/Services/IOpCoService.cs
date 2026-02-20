using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IOpCoService
    {
        Task<IEnumerable<OpCoDto>> GetAllOpCosAsync();
        Task<OpCoDto?> GetOpCoByIdAsync(int id);
        Task<OpCoDto> CreateOpCoAsync(CreateOpCoDto createOpCoDto, string createdBy);
        Task<OpCoDto> UpdateOpCoAsync(int id, UpdateOpCoDto updateOpCoDto, string modifiedBy);
        Task<bool> DeleteOpCoAsync(int id);
        Task<bool> ActivateOpCoAsync(int id, string modifiedBy);
        Task<bool> DeactivateOpCoAsync(int id, string modifiedBy);
        Task<IEnumerable<OpCoDto>> GetOpCosByDistributorIdAsync(int distributorId);
        Task<IEnumerable<OpCoDto>> GetOpCosByDistributorIdsAsync(IEnumerable<int> distributorIds);
        Task<IEnumerable<OpCoDto>> GetOpCosByStatusAsync(int status);
        Task<OpCoDto?> GetOpCoByRemoteReferenceCodeAsync(string remoteReferenceCode);
        Task<(IEnumerable<OpCoDto> OpCos, int TotalCount)> SearchOpCosAsync(string? searchTerm, int? distributorId = null, int? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? remoteReferenceCode = null);
    }
}
