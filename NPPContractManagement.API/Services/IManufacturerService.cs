using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync();
        Task<(IEnumerable<ManufacturerDto> Manufacturers, int TotalCount)> SearchManufacturersAsync(
            string searchTerm,
            int? status = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc",
            string? state = null,
            int? primaryBrokerId = null);
        Task<ManufacturerDto?> GetManufacturerByIdAsync(int id);
        Task<ManufacturerDto> CreateManufacturerAsync(CreateManufacturerDto createManufacturerDto, string createdBy);
        Task<ManufacturerDto> UpdateManufacturerAsync(int id, UpdateManufacturerDto updateManufacturerDto, string modifiedBy);
        Task<bool> DeleteManufacturerAsync(int id);
        Task<bool> ActivateManufacturerAsync(int id, string modifiedBy);
        Task<bool> DeactivateManufacturerAsync(int id, string modifiedBy);
    }
}
