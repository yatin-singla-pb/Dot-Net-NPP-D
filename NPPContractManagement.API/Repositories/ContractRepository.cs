using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }



        public async Task<IEnumerable<Contract>> GetByManufacturerIdAsync(int manufacturerId)
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.ManufacturerId == manufacturerId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status)
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByDistributorIdAsync(int distributorId)
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.ContractDistributors.Any(cd => cd.DistributorId == distributorId))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByOpCoIdAsync(int opCoId)
        {
            return await _context.Contracts

                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .Where(c => c.ContractOpCos.Any(co => co.OpCoId == opCoId))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetByIndustryIdAsync(int industryId)
        {
            return await _context.Contracts

                .Include(c => c.ContractIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Where(c => c.ContractIndustries.Any(ci => ci.IndustryId == industryId))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetSuspendedContractsAsync()
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.IsSuspended)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsForPerformanceAsync()
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.SendToPerformance)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetExpiringContractsAsync(DateTime beforeDate)
        {
            return await _context.Contracts

                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Where(c => c.EndDate <= beforeDate)
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetExpiringContractsWithoutProposalsAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.Contracts
                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .Include(c => c.ContractIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Where(c => c.EndDate <= thresholdDate
                    && c.EndDate >= DateTime.UtcNow
                    && c.ProposalId == null
                    && !c.IsSuspended)
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> SearchAsync(string searchTerm, int? manufacturerId = null, ContractStatus? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, IEnumerable<int>? allowedManufacturerIds = null)
        {
            var query = _context.Contracts
                .Include(c => c.Proposal)
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    (c.Name != null && c.Name.Contains(searchTerm)) ||
                    (c.InternalNotes != null && c.InternalNotes.Contains(searchTerm)) ||
                    (c.ForeignContractId != null && c.ForeignContractId.Contains(searchTerm))
                );
            }

            if (distributorId.HasValue)
            {
                query = query.Where(c => c.ContractDistributors.Any(cd => cd.DistributorId == distributorId.Value));
            }

            if (industryId.HasValue)
            {
                query = query.Where(c => c.ContractIndustries.Any(ci => ci.IndustryId == industryId.Value));
            }

            if (isSuspended.HasValue)
            {
                query = query.Where(c => c.IsSuspended == isSuspended.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            return await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(int? manufacturerId = null, ContractStatus? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, IEnumerable<int>? allowedManufacturerIds = null)
        {
            var query = _context.Contracts.AsQueryable();

            if (distributorId.HasValue)
            {
                query = query.Where(c => c.ContractDistributors.Any(cd => cd.DistributorId == distributorId.Value));
            }

            if (industryId.HasValue)
            {
                query = query.Where(c => c.ContractIndustries.Any(ci => ci.IndustryId == industryId.Value));
            }

            if (isSuspended.HasValue)
            {
                query = query.Where(c => c.IsSuspended == isSuspended.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<bool> ValidateUniqueConstraintAsync(int manufacturerId, DateTime startDate, DateTime endDate, List<int> industryIds, List<int> opCoIds, int? excludeId = null)
        {
            var query = _context.Contracts
                .Where(c => c.ManufacturerId == manufacturerId &&
                           c.StartDate <= endDate &&
                           c.EndDate >= startDate);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            // Check for overlapping contracts with same industries and op-cos
            var existingContracts = await query
                .Include(c => c.ContractIndustries)
                .Include(c => c.ContractOpCos)
                .ToListAsync();

            foreach (var contract in existingContracts)
            {
                var contractIndustryIds = contract.ContractIndustries.Select(ci => ci.IndustryId).ToList();
                var contractOpCoIds = contract.ContractOpCos.Select(co => co.OpCoId).ToList();

                // Check if there's any overlap in industries and op-cos
                if (industryIds.Any(id => contractIndustryIds.Contains(id)) &&
                    opCoIds.Any(id => contractOpCoIds.Contains(id)))
                {
                    return false; // Constraint violation
                }
            }

            return true; // No constraint violation
        }

        public override async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Proposal)
                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .Include(c => c.ContractIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .Include(c => c.ContractProducts)
                    .ThenInclude(cp => cp.Product)
                .Include(c => c.ContractVersions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Proposal)
                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }
    }
}
