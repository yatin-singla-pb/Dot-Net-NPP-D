using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IIndustryRepository : IRepository<Industry>
    {
        Task<Industry?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<IEnumerable<Industry>> GetByStatusAsync(IndustryStatus status);
        Task<IEnumerable<Industry>> SearchAsync(string searchTerm, IndustryStatus? status = null, int page = 1, int pageSize = 10);
        Task<int> GetCountAsync(IndustryStatus? status = null);
        Task<IEnumerable<Industry>> GetActiveIndustriesAsync();
    }
}
