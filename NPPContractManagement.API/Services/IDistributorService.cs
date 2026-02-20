using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IDistributorService
    {
        Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync();
        Task<DistributorDto?> GetDistributorByIdAsync(int id);
        Task<DistributorDto> CreateDistributorAsync(CreateDistributorDto createDistributorDto, string createdBy);
        Task<DistributorDto> UpdateDistributorAsync(int id, UpdateDistributorDto updateDistributorDto, string modifiedBy);
        Task<bool> DeleteDistributorAsync(int id);
        Task<bool> ActivateDistributorAsync(int id, string modifiedBy);
        Task<bool> DeactivateDistributorAsync(int id, string modifiedBy);

        // Search with pagination similar to Industries
        Task<(IEnumerable<DistributorDto> Distributors, int TotalCount)> SearchDistributorsAsync(string searchTerm, int? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", bool? receiveContractProposal = null, string? state = null);
    }
}
