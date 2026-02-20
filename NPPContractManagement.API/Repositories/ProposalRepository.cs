using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Domain.Proposals.Entities;

namespace NPPContractManagement.API.Repositories
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ProposalRepository> _logger;
        public ProposalRepository(ApplicationDbContext db, ILogger<ProposalRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<(IEnumerable<Proposal> Items, int TotalCount)> QueryAsync(
            string? search,
            int page,
            int pageSize,
            IEnumerable<int>? manufacturerIds = null,
            int? proposalStatusId = null,
            int? proposalTypeId = null,
            int? manufacturerId = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            DateTime? endDateFrom = null,
            DateTime? endDateTo = null,
            DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null,
            int? idFrom = null,
            int? idTo = null,
            string? sortBy = null,
            string? sortDirection = null)
        {
            var q = _db.Proposals
                .Include(p => p.ProposalStatus)
                .Include(p => p.Manufacturer)
                .AsQueryable();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("[Repo] Query start: search='{search}', page={page}, pageSize={pageSize}, mIds=[{ids}], statusId={statusId}, typeId={typeId}, manufacturerId={manId}, sortBy={sortBy}, sortDir={sortDir}",
                    search, page, pageSize, manufacturerIds == null ? "" : string.Join(',', manufacturerIds), proposalStatusId, proposalTypeId, manufacturerId, sortBy, sortDirection);
            }

            // Manufacturer filter (server-side)
            if (manufacturerIds != null && manufacturerIds.Any())
            {
                var set = manufacturerIds.ToHashSet();
                q = q.Where(p => p.ManufacturerId.HasValue && set.Contains(p.ManufacturerId.Value));
            }

            if (manufacturerId.HasValue)
            {
                q = q.Where(p => p.ManufacturerId == manufacturerId.Value);
            }

            if (proposalStatusId.HasValue)
            {
                q = q.Where(p => p.ProposalStatusId == proposalStatusId.Value);
            }

            if (proposalTypeId.HasValue)
            {
                q = q.Where(p => p.ProposalTypeId == proposalTypeId.Value);
            }

            if (startDateFrom.HasValue)
            {
                q = q.Where(p => p.StartDate != null && p.StartDate >= startDateFrom.Value);
            }
            if (startDateTo.HasValue)
            {
                q = q.Where(p => p.StartDate != null && p.StartDate <= startDateTo.Value);
            }
            if (endDateFrom.HasValue)
            {
                q = q.Where(p => p.EndDate != null && p.EndDate >= endDateFrom.Value);
            }
            if (endDateTo.HasValue)
            {
                q = q.Where(p => p.EndDate != null && p.EndDate <= endDateTo.Value);
            }

            if (createdDateFrom.HasValue)
            {
                q = q.Where(p => p.CreatedDate >= createdDateFrom.Value);
            }
            if (createdDateTo.HasValue)
            {
                q = q.Where(p => p.CreatedDate <= createdDateTo.Value);
            }
            if (idFrom.HasValue)
            {
                q = q.Where(p => p.Id >= idFrom.Value);
            }
            if (idTo.HasValue)
            {
                q = q.Where(p => p.Id <= idTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(p => p.Title.Contains(term));
            }

            // Sorting
            var dirDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch ((sortBy ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "id":
                    q = dirDesc ? q.OrderByDescending(p => p.Id) : q.OrderBy(p => p.Id);
                    break;
                case "title":
                    q = dirDesc ? q.OrderByDescending(p => p.Title) : q.OrderBy(p => p.Title);
                    break;
                case "startdate":
                    q = dirDesc ? q.OrderByDescending(p => p.StartDate) : q.OrderBy(p => p.StartDate);
                    break;
                case "enddate":
                    q = dirDesc ? q.OrderByDescending(p => p.EndDate) : q.OrderBy(p => p.EndDate);
                    break;
                case "createddate":
                    q = dirDesc ? q.OrderByDescending(p => p.CreatedDate) : q.OrderBy(p => p.CreatedDate);
                    break;
                default:
                    q = q.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var sample = items.FirstOrDefault();
                var statuses = items.Select(i => i.ProposalStatusId).Distinct().OrderBy(x => x).ToArray();
                _logger.LogDebug("[Repo] Query end: total={total}, batchCount={count}, sampleId={sid}, sampleStatusId={ssid}, statuses=[{statuses}]",
                    total, items.Count, sample?.Id, sample?.ProposalStatusId, string.Join(',', statuses));
            }

            return (items, total);
        }

        public async Task<Proposal?> GetByIdAsync(int id)
        {
            return await _db.Proposals
                .Include(p => p.ProposalStatus)
                .Include(p => p.Manufacturer)
                .Include(p => p.Products).ThenInclude(pp => pp.Product)
                .Include(p => p.Distributors).ThenInclude(d => d.Distributor)
                .Include(p => p.Industries).ThenInclude(i => i.Industry)
                .Include(p => p.Opcos).ThenInclude(o => o.OpCo)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Proposal> AddAsync(Proposal entity)
        {
            _db.Proposals.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Proposal entity)
        {
            _db.Proposals.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Proposals.FindAsync(id);
            if (entity != null)
            {
                entity.DeletedAt = DateTime.UtcNow;
                entity.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }
    }
}

