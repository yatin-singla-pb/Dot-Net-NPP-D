using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ManufacturerRepository : Repository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Manufacturer?> GetByNameAsync(string name)
        {
            return await _context.Manufacturers
                .Include(m => m.Products)
                .Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser)
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Manufacturers.Where(m => m.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Manufacturer>> GetByStatusAsync(ManufacturerStatus status)
        {
            return await _context.Manufacturers
                .Where(m => m.Status == status)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Manufacturer>> SearchAsync(string searchTerm, ManufacturerStatus? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? state = null, int? primaryBrokerId = null)
        {
            var query = _context.Manufacturers.Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(m => m.Id == searchId);
                }
                else
                {
                    // Fuzzy name match
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(m => m.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(m => m.State != null && m.State.ToLower() == state.ToLower());
            }

            if (primaryBrokerId.HasValue)
            {
                query = query.Where(m => m.PrimaryBrokerId == primaryBrokerId.Value);
            }

            // Only active records for listings
            query = query.Where(m => m.IsActive);

            // Sorting
            var directionDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch ((sortBy ?? "Name").ToLower())
            {
                case "id":
                    query = directionDesc ? query.OrderByDescending(m => m.Id) : query.OrderBy(m => m.Id);
                    break;
                case "name":
                    query = directionDesc ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name);
                    break;
                case "address":
                    query = directionDesc ? query.OrderByDescending(m => m.Address) : query.OrderBy(m => m.Address);
                    break;
                case "phonenumber":
                    query = directionDesc ? query.OrderByDescending(m => m.PhoneNumber) : query.OrderBy(m => m.PhoneNumber);
                    break;
                case "status":
                    query = directionDesc ? query.OrderByDescending(m => m.Status) : query.OrderBy(m => m.Status);
                    break;
                default:
                    query = directionDesc ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name);
                    break;
            }

            return await query
                .Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string searchTerm, ManufacturerStatus? status = null, string? state = null, int? primaryBrokerId = null)
        {
            var query = _context.Manufacturers.Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(m => m.Id == searchId);
                }
                else
                {
                    // Fuzzy name match
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(m => m.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(m => m.State != null && m.State.ToLower() == state.ToLower());
            }

            if (primaryBrokerId.HasValue)
            {
                query = query.Where(m => m.PrimaryBrokerId == primaryBrokerId.Value);
            }

            // Only active records count for listings
            query = query.Where(m => m.IsActive);

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Manufacturer>> GetActiveManufacturersAsync()
        {
            return await _context.Manufacturers
                .Where(m => m.Status == ManufacturerStatus.Active && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public override async Task<Manufacturer?> GetByIdAsync(int id)
        {
            return await _context.Manufacturers
                .Include(m => m.Products)
                .Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            return await _context.Manufacturers
                .Include(m => m.PrimaryBroker)
                .Include(m => m.ContactPersonUser)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }
    }
}
