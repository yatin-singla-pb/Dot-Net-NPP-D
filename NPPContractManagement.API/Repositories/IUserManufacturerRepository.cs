using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPPContractManagement.API.Repositories
{
    public interface IUserManufacturerRepository
    {
        Task<IReadOnlyList<int>> GetManufacturerIdsForUserAsync(int userId);
        Task<IReadOnlyList<int>> GetUserIdsForManufacturerAsync(int manufacturerId);
        Task SyncUserManufacturersAsync(int userId, IEnumerable<int> manufacturerIds, string assignedBy);
    }
}

