using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class CustomerAccountRepository : Repository<CustomerAccount>, ICustomerAccountRepository
    {
        public CustomerAccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CustomerAccount>> GetByMemberAccountIdAsync(int memberAccountId)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .Where(c => c.MemberAccountId == memberAccountId)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerAccount>> GetByDistributorIdAsync(int distributorId)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .Where(c => c.DistributorId == distributorId)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerAccount>> GetByOpCoIdAsync(int opCoId)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .Where(c => c.OpCoId == opCoId)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerAccount>> GetByStatusAsync(CustomerAccountStatus status)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .Where(c => c.Status == status)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<CustomerAccount?> GetByCustomerAccountNumberAsync(int distributorId, string customerAccountNumber)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .FirstOrDefaultAsync(c => c.DistributorId == distributorId && c.CustomerAccountNumber == customerAccountNumber);
        }

        public async Task<bool> ExistsByCustomerAccountNumberAsync(int distributorId, string customerAccountNumber, int? excludeId = null)
        {
            var query = _context.CustomerAccounts
                .Where(c => c.DistributorId == distributorId && c.CustomerAccountNumber == customerAccountNumber);
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<CustomerAccount>> SearchAsync(string searchTerm, int? memberAccountId = null, int? distributorId = null, int? opCoId = null, CustomerAccountStatus? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null, int page = 1, int pageSize = 10)
        {
            var query = _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm, out int searchId))
                {
                    query = query.Where(c => c.Id == searchId);
                }
                else
                {
                    // Exact Account # match or fuzzy name match
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(c =>
                        c.CustomerAccountNumber.ToLower() == lowerSearchTerm ||
                        c.CustomerName.ToLower().Contains(lowerSearchTerm));
                }
            }

            if (memberAccountId.HasValue)
            {
                query = query.Where(c => c.MemberAccountId == memberAccountId.Value);
            }

            if (distributorId.HasValue)
            {
                query = query.Where(c => c.DistributorId == distributorId.Value);
            }

            if (opCoId.HasValue)
            {
                query = query.Where(c => c.OpCoId == opCoId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (industryId.HasValue)
            {
                query = query.Where(c => c.MemberAccount.IndustryId == industryId.Value);
            }

            if (association.HasValue)
            {
                query = query.Where(c => (int)c.Association == association.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate.HasValue && c.StartDate.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate.HasValue && c.EndDate.Value.Date <= endDate.Value.Date);
            }

            if (tracsAccess.HasValue)
            {
                query = query.Where(c => c.TRACSAccess == tracsAccess.Value);
            }

            if (toEntegra.HasValue)
            {
                query = query.Where(c => c.ToEntegra == toEntegra.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                var lowerState = state.ToLower();
                query = query.Where(c => c.State != null && c.State.ToLower().Contains(lowerState));
            }

            return await query
                .OrderBy(c => c.CustomerName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string searchTerm = "", int? memberAccountId = null, int? distributorId = null, int? opCoId = null, CustomerAccountStatus? status = null, bool? isActive = null, int? industryId = null, int? association = null, DateTime? startDate = null, DateTime? endDate = null, bool? tracsAccess = null, bool? toEntegra = null, string? state = null)
        {
            var query = _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm, out int searchId))
                {
                    query = query.Where(c => c.Id == searchId);
                }
                else
                {
                    // Exact Account # match or fuzzy name match
                    var lowerSearchTerm = searchTerm.ToLower();
                    query = query.Where(c =>
                        c.CustomerAccountNumber.ToLower() == lowerSearchTerm ||
                        c.CustomerName.ToLower().Contains(lowerSearchTerm));
                }
            }

            if (memberAccountId.HasValue)
            {
                query = query.Where(c => c.MemberAccountId == memberAccountId.Value);
            }

            if (distributorId.HasValue)
            {
                query = query.Where(c => c.DistributorId == distributorId.Value);
            }

            if (opCoId.HasValue)
            {
                query = query.Where(c => c.OpCoId == opCoId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (industryId.HasValue)
            {
                query = query.Where(c => c.MemberAccount.IndustryId == industryId.Value);
            }

            if (association.HasValue)
            {
                query = query.Where(c => (int)c.Association == association.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate.HasValue && c.StartDate.Value.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate.HasValue && c.EndDate.Value.Date <= endDate.Value.Date);
            }

            if (tracsAccess.HasValue)
            {
                query = query.Where(c => c.TRACSAccess == tracsAccess.Value);
            }

            if (toEntegra.HasValue)
            {
                query = query.Where(c => c.ToEntegra == toEntegra.Value);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                var lowerState = state.ToLower();
                query = query.Where(c => c.State != null && c.State.ToLower().Contains(lowerState));
            }

            return await query.CountAsync();
        }


        public override async Task<CustomerAccount?> GetByIdAsync(int id)
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                    .ThenInclude(m => m.Industry)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<CustomerAccount>> GetAllAsync()
        {
            return await _context.CustomerAccounts
                .Include(c => c.MemberAccount)
                .Include(c => c.Distributor)
                .Include(c => c.OpCo)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }
    }
}
