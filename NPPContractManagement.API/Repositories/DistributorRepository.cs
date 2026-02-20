using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class DistributorRepository : Repository<Distributor>, IDistributorRepository
    {
        public DistributorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Distributor?> GetByNameAsync(string name)
        {
            return await _context.Distributors
                .Include(d => d.OpCos)
                .Include(d => d.CustomerAccounts)
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Distributors.Where(d => d.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Distributor>> GetByStatusAsync(DistributorStatus status)
        {
            return await _context.Distributors
                .Where(d => d.Status == status)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Distributor>> GetByReceiveContractProposalAsync(bool receiveContractProposal)
        {
            return await _context.Distributors
                .Where(d => d.ReceiveContractProposal == receiveContractProposal)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Distributor>> SearchAsync(string searchTerm, DistributorStatus? status = null, bool? receiveContractProposal = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? state = null)
        {
            var query = _context.Distributors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(d => d.Id == searchId);
                }
                else
                {
                    // Fuzzy name match (contains)
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(d => d.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (receiveContractProposal.HasValue)
            {
                query = query.Where(d => d.ReceiveContractProposal == receiveContractProposal.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(d => d.State != null && d.State.ToLower() == state.ToLower());
            }

            // Only active records for listings
            query = query.Where(d => d.IsActive);

            // Sorting
            var directionDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch ((sortBy ?? "Name").ToLower())
            {
                case "id":
                    query = directionDesc ? query.OrderByDescending(d => d.Id) : query.OrderBy(d => d.Id);
                    break;
                case "name":
                    query = directionDesc ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);
                    break;
                case "address":
                    query = directionDesc ? query.OrderByDescending(d => d.Address) : query.OrderBy(d => d.Address);
                    break;
                case "phonenumber":
                    query = directionDesc ? query.OrderByDescending(d => d.PhoneNumber) : query.OrderBy(d => d.PhoneNumber);
                    break;
                case "status":
                    query = directionDesc ? query.OrderByDescending(d => d.Status) : query.OrderBy(d => d.Status);
                    break;
                default:
                    query = directionDesc ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);
                    break;
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string searchTerm, DistributorStatus? status = null, bool? receiveContractProposal = null, string? state = null)
        {
            var query = _context.Distributors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(d => d.Id == searchId);
                }
                else
                {
                    // Fuzzy name match (contains)
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(d => d.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (receiveContractProposal.HasValue)
            {
                query = query.Where(d => d.ReceiveContractProposal == receiveContractProposal.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(d => d.State != null && d.State.ToLower() == state.ToLower());
            }

            // Only active records count for listings
            query = query.Where(d => d.IsActive);

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Distributor>> GetActiveDistributorsAsync()
        {
            return await _context.Distributors
                .Where(d => d.Status == DistributorStatus.Active && d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public override async Task<Distributor?> GetByIdAsync(int id)
        {
            return await _context.Distributors
                .Include(d => d.OpCos)
                .Include(d => d.CustomerAccounts)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public override async Task<IEnumerable<Distributor>> GetAllAsync()
        {
            return await _context.Distributors
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
