using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IManufacturerRepository : IRepository<Manufacturer>
    {
        Task<Manufacturer?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<IEnumerable<Manufacturer>> GetByStatusAsync(ManufacturerStatus status);
        Task<IEnumerable<Manufacturer>> SearchAsync(
            string searchTerm,
            ManufacturerStatus? status = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc",
            string? state = null,
            int? primaryBrokerId = null);
        Task<int> GetCountAsync(string searchTerm, ManufacturerStatus? status = null, string? state = null, int? primaryBrokerId = null);
        Task<IEnumerable<Manufacturer>> GetActiveManufacturersAsync();
    }
}
