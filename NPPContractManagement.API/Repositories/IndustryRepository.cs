using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class IndustryRepository : Repository<Industry>, IIndustryRepository
    {
        public IndustryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Industry?> GetByNameAsync(string name)
        {
            return await _context.Industries
                .Include(i => i.Users)
                .Include(i => i.MemberAccounts)
                .FirstOrDefaultAsync(i => i.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Industries.Where(i => i.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Industry>> GetByStatusAsync(IndustryStatus status)
        {
            return await _context.Industries
                .Where(i => i.Status == status)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Industry>> SearchAsync(string searchTerm, IndustryStatus? status = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Industries.AsQueryable();

            // Always filter by IsActive = true
            query = query.Where(i => i.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try exact ID match first
                if (int.TryParse(searchTerm, out int searchId))
                {
                    query = query.Where(i => i.Id == searchId || i.Name.Contains(searchTerm));
                }
                else
                {
                    // Fuzzy name match only
                    query = query.Where(i => i.Name.Contains(searchTerm));
                }
            }

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            return await query
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(IndustryStatus? status = null)
        {
            var query = _context.Industries.AsQueryable();

            // Always filter by IsActive = true
            query = query.Where(i => i.IsActive);

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Industry>> GetActiveIndustriesAsync()
        {
            return await _context.Industries
                .Where(i => i.Status == IndustryStatus.Active && i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public override async Task<Industry?> GetByIdAsync(int id)
        {
            return await _context.Industries
                .Include(i => i.Users)
                .Include(i => i.MemberAccounts)
                .Include(i => i.ContractIndustries)
                    .ThenInclude(ci => ci.Contract)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public override async Task<IEnumerable<Industry>> GetAllAsync()
        {
            return await _context.Industries
                .OrderBy(i => i.Name)
                .ToListAsync();
        }
    }
}
