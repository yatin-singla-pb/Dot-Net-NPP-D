using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public interface IIndustryService
    {
        Task<IEnumerable<IndustryDto>> GetAllIndustriesAsync();
        Task<PaginatedResult<IndustryDto>> GetPaginatedIndustriesAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? searchTerm = null);
        Task<IndustryDto?> GetIndustryByIdAsync(int id);
        Task<IndustryDto> CreateIndustryAsync(CreateIndustryDto createIndustryDto, string createdBy);
        Task<IndustryDto> UpdateIndustryAsync(int id, UpdateIndustryDto updateIndustryDto, string modifiedBy);
        Task<bool> DeleteIndustryAsync(int id);
        Task<bool> ActivateIndustryAsync(int id, string modifiedBy);
        Task<bool> DeactivateIndustryAsync(int id, string modifiedBy);
        Task<IndustryDto?> GetIndustryByNameAsync(string name);
        Task<IEnumerable<IndustryDto>> GetIndustriesByStatusAsync(int status);
        Task<IEnumerable<IndustryDto>> GetActiveIndustriesAsync();
        Task<(IEnumerable<IndustryDto> Industries, int TotalCount)> SearchIndustriesAsync(string searchTerm, int? status = null, int page = 1, int pageSize = 10);
    }
}
