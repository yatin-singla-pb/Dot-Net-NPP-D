using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUserIdAsync(string userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithRolesAsync(int id);
        Task<User?> GetUserWithRolesByUserIdAsync(string userId);
        Task<bool> IsUserIdTakenAsync(string userId);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsEmailTakenAsync(string email, int excludeUserId);

        Task<IEnumerable<User>> SearchAsync(string searchTerm, bool? isActive = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", AccountStatus? accountStatus = null);
        Task<int> GetCountAsync(string searchTerm, bool? isActive = null, AccountStatus? accountStatus = null);
    }
}
