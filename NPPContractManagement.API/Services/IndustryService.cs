using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class IndustryService : IIndustryService
    {
        private readonly IIndustryRepository _industryRepository;

        public IndustryService(IIndustryRepository industryRepository)
        {
            _industryRepository = industryRepository;
        }

        public async Task<IEnumerable<IndustryDto>> GetAllIndustriesAsync()
        {
            var industries = await _industryRepository.GetAllAsync();
            return industries.Select(MapToDto);
        }

        public async Task<PaginatedResult<IndustryDto>> GetPaginatedIndustriesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc",
            string? searchTerm = null)
        {
            var result = await _industryRepository.GetPaginatedAsync(
                pageNumber, pageSize, sortBy, sortDirection, searchTerm);

            return new PaginatedResult<IndustryDto>
            {
                Items = result.Items.Select(MapToDto),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }

        public async Task<IndustryDto?> GetIndustryByIdAsync(int id)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            return industry != null ? MapToDto(industry) : null;
        }

        public async Task<IndustryDto> CreateIndustryAsync(CreateIndustryDto createIndustryDto, string createdBy)
        {
            // Validate unique name
            var exists = await _industryRepository.ExistsByNameAsync(createIndustryDto.Name);
            if (exists)
            {
                throw new ArgumentException("Industry name already exists", nameof(createIndustryDto.Name));
            }

            var industry = new Industry
            {
                Name = createIndustryDto.Name,
                Description = createIndustryDto.Description,
                Status = (IndustryStatus)createIndustryDto.Status,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var createdIndustry = await _industryRepository.AddAsync(industry);
            return MapToDto(createdIndustry);
        }

        public async Task<IndustryDto> UpdateIndustryAsync(int id, UpdateIndustryDto updateIndustryDto, string modifiedBy)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            if (industry == null)
            {
                throw new ArgumentException("Industry not found", nameof(id));
            }

            // Validate unique name
            var exists = await _industryRepository.ExistsByNameAsync(updateIndustryDto.Name, id);
            if (exists)
            {
                throw new ArgumentException("Industry name already exists", nameof(updateIndustryDto.Name));
            }

            industry.Name = updateIndustryDto.Name;
            industry.Description = updateIndustryDto.Description;
            industry.Status = (IndustryStatus)updateIndustryDto.Status;
            industry.IsActive = updateIndustryDto.IsActive;
            industry.ModifiedDate = DateTime.UtcNow;
            industry.ModifiedBy = modifiedBy;

            var updatedIndustry = await _industryRepository.UpdateAsync(industry);
            return MapToDto(updatedIndustry);
        }

        public async Task<bool> DeleteIndustryAsync(int id)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            if (industry == null)
            {
                return false;
            }

            // Check if industry has users or member accounts
            if (industry.Users?.Any() == true || industry.MemberAccounts?.Any() == true)
            {
                throw new InvalidOperationException("Cannot delete industry with existing users or member accounts");
            }

            await _industryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateIndustryAsync(int id, string modifiedBy)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            if (industry == null)
            {
                return false;
            }

            industry.IsActive = true;
            industry.Status = IndustryStatus.Active;
            industry.ModifiedDate = DateTime.UtcNow;
            industry.ModifiedBy = modifiedBy;

            await _industryRepository.UpdateAsync(industry);
            return true;
        }

        public async Task<bool> DeactivateIndustryAsync(int id, string modifiedBy)
        {
            var industry = await _industryRepository.GetByIdAsync(id);
            if (industry == null)
            {
                return false;
            }

            industry.IsActive = false;
            industry.Status = IndustryStatus.Inactive;
            industry.ModifiedDate = DateTime.UtcNow;
            industry.ModifiedBy = modifiedBy;

            await _industryRepository.UpdateAsync(industry);
            return true;
        }

        public async Task<IndustryDto?> GetIndustryByNameAsync(string name)
        {
            var industry = await _industryRepository.GetByNameAsync(name);
            return industry != null ? MapToDto(industry) : null;
        }

        public async Task<IEnumerable<IndustryDto>> GetIndustriesByStatusAsync(int status)
        {
            var industries = await _industryRepository.GetByStatusAsync((IndustryStatus)status);
            return industries.Select(MapToDto);
        }

        public async Task<IEnumerable<IndustryDto>> GetActiveIndustriesAsync()
        {
            var industries = await _industryRepository.GetActiveIndustriesAsync();
            return industries.Select(MapToDto);
        }

        public async Task<(IEnumerable<IndustryDto> Industries, int TotalCount)> SearchIndustriesAsync(string searchTerm, int? status = null, int page = 1, int pageSize = 10)
        {
            var industries = await _industryRepository.SearchAsync(searchTerm, status.HasValue ? (IndustryStatus)status.Value : null, page, pageSize);
            var totalCount = await _industryRepository.GetCountAsync(status.HasValue ? (IndustryStatus)status.Value : null);

            return (industries.Select(MapToDto), totalCount);
        }

        private static IndustryDto MapToDto(Industry industry)
        {
            return new IndustryDto
            {
                Id = industry.Id,
                Name = industry.Name,
                Description = industry.Description,
                Status = (int)industry.Status,
                StatusName = industry.Status.ToString(),
                IsActive = industry.IsActive,
                CreatedDate = industry.CreatedDate,
                ModifiedDate = industry.ModifiedDate,
                CreatedBy = industry.CreatedBy,
                ModifiedBy = industry.ModifiedBy
            };
        }
    }
}
