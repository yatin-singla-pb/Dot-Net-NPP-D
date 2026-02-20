using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IDistributorProductCodeService
    {
        Task<(IEnumerable<DistributorProductCodeDto> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            IEnumerable<int>? distributorIds = null,
            IEnumerable<int>? productIds = null,
            int? productStatus = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc");

        Task<DistributorProductCodeDto?> GetByIdAsync(int id);
        Task<DistributorProductCodeDto> CreateAsync(CreateDistributorProductCodeDto dto, string createdBy);
        Task<DistributorProductCodeDto> UpdateAsync(int id, UpdateDistributorProductCodeDto dto, string modifiedBy);
        Task<bool> DeleteAsync(int id);
    }
}

