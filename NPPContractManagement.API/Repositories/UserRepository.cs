using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithRolesAsync(int id)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserWithRolesByUserIdAsync(string userId)
        {

            

            //Console.WriteLine(_dbSet.ToQueryString());
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> IsUserIdTakenAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            return await _dbSet.AnyAsync(u => u.UserId != null && u.UserId == userId);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailTakenAsync(string email, int excludeUserId)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
        }

        public async Task<IEnumerable<User>> SearchAsync(string searchTerm, bool? isActive = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", AccountStatus? accountStatus = null)
        {
            var query = _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.Trim().ToLower();
                var isNumeric = int.TryParse(searchTerm.Trim(), out int searchId);

                query = query.Where(u =>
                    // Exact ID match
                    (isNumeric && u.Id == searchId) ||
                    // Exact username match
                    (u.UserId != null && u.UserId.ToLower() == lowerSearchTerm) ||
                    // Fuzzy name match
                    (u.FirstName + " " + u.LastName).ToLower().Contains(lowerSearchTerm) ||
                    // Fuzzy email match
                    u.Email.ToLower().Contains(lowerSearchTerm));
            }

            if (accountStatus.HasValue)
            {
                query = query.Where(u => u.AccountStatus == accountStatus.Value);
            }
            else if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // Sorting
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? nameof(User.Id) : sortBy;
            var isDesc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
            query = sortBy switch
            {
                nameof(User.UserId) => isDesc ? query.OrderByDescending(u => u.UserId) : query.OrderBy(u => u.UserId),
                nameof(User.FirstName) => isDesc ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                nameof(User.LastName) => isDesc ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                nameof(User.Email) => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => isDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string searchTerm, bool? isActive = null, AccountStatus? accountStatus = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.Trim().ToLower();
                var isNumeric = int.TryParse(searchTerm.Trim(), out int searchId);

                query = query.Where(u =>
                    // Exact ID match
                    (isNumeric && u.Id == searchId) ||
                    // Exact username match
                    (u.UserId != null && u.UserId.ToLower() == lowerSearchTerm) ||
                    // Fuzzy name match
                    (u.FirstName + " " + u.LastName).ToLower().Contains(lowerSearchTerm) ||
                    // Fuzzy email match
                    u.Email.ToLower().Contains(lowerSearchTerm));
            }

            if (accountStatus.HasValue)
            {
                query = query.Where(u => u.AccountStatus == accountStatus.Value);
            }
            else if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            return await query.CountAsync();
        }
    }
}
