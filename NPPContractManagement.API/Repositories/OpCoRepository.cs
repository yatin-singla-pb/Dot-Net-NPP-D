using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class OpCoRepository : Repository<OpCo>, IOpCoRepository
    {
        public OpCoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OpCo>> GetByDistributorIdAsync(int distributorId)
        {
            return await _context.OpCos
                .Include(o => o.Distributor)
                .Where(o => o.DistributorId == distributorId)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<OpCo>> GetByStatusAsync(OpCoStatus status)
        {
            return await _context.OpCos
                .Include(o => o.Distributor)
                .Where(o => o.Status == status)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<OpCo?> GetByRemoteReferenceCodeAsync(string remoteReferenceCode)
        {
            return await _context.OpCos
                .Include(o => o.Distributor)
                .FirstOrDefaultAsync(o => o.RemoteReferenceCode == remoteReferenceCode);
        }

        public async Task<bool> ExistsByRemoteReferenceCodeAsync(string remoteReferenceCode, int? excludeId = null)
        {
            var query = _context.OpCos.Where(o => o.RemoteReferenceCode == remoteReferenceCode);

            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<OpCo>> SearchAsync(string? searchTerm, int? distributorId = null, OpCoStatus? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? remoteReferenceCode = null)
        {
            var query = _context.OpCos
                .Include(o => o.Distributor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(o => o.Id == searchId);
                }
                else
                {
                    // Fuzzy name match (contains)
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(o => o.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (distributorId.HasValue)
            {
                query = query.Where(o => o.DistributorId == distributorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(remoteReferenceCode))
            {
                query = query.Where(o => o.RemoteReferenceCode != null && o.RemoteReferenceCode.Contains(remoteReferenceCode));
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var dirDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                query = sortBy.ToLower() switch
                {
                    "id" => dirDesc ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),
                    "name" => dirDesc ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name),
                    "remotereferencecode" => dirDesc ? query.OrderByDescending(o => o.RemoteReferenceCode) : query.OrderBy(o => o.RemoteReferenceCode),
                    "distributorname" => dirDesc ? query.OrderByDescending(o => o.Distributor.Name) : query.OrderBy(o => o.Distributor.Name),
                    "status" => dirDesc ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                    _ => dirDesc ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name)
                };
            }
            else
            {
                query = query.OrderBy(o => o.Name);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string? searchTerm = null, int? distributorId = null, OpCoStatus? status = null, string? remoteReferenceCode = null)
        {
            var query = _context.OpCos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(o => o.Id == searchId);
                }
                else
                {
                    // Fuzzy name match (contains)
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(o => o.Name.ToLower().Contains(lowerTerm));
                }
            }

            if (distributorId.HasValue)
            {
                query = query.Where(o => o.DistributorId == distributorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(remoteReferenceCode))
            {
                query = query.Where(o => o.RemoteReferenceCode != null && o.RemoteReferenceCode.Contains(remoteReferenceCode));
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public override async Task<OpCo?> GetByIdAsync(int id)
        {
            return await _context.OpCos
                .Include(o => o.Distributor)
                .Include(o => o.CustomerAccounts)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public override async Task<IEnumerable<OpCo>> GetAllAsync()
        {
            return await _context.OpCos
                .Include(o => o.Distributor)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<OpCo>> GetByDistributorIdsAsync(IEnumerable<int> distributorIds)
        {
            var ids = (distributorIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (ids.Count == 0) return new List<OpCo>();
            return await _context.OpCos
                .Include(o => o.Distributor)
                .Where(o => ids.Contains(o.DistributorId))
                .OrderBy(o => o.Name)
                .ToListAsync();
        }
    }
}