using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class MemberAccountRepository : Repository<MemberAccount>, IMemberAccountRepository
    {
        public MemberAccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<MemberAccount?> GetByMemberNumberAsync(string memberNumber)
        {
            return await _context.MemberAccounts
                .Include(m => m.Industry)
                .Include(m => m.CustomerAccounts)
                .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);
        }

        public async Task<bool> ExistsByMemberNumberAsync(string memberNumber, int? excludeId = null)
        {
            var query = _context.MemberAccounts.Where(m => m.MemberNumber == memberNumber);
            
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<MemberAccount>> GetByIndustryIdAsync(int industryId)
        {
            return await _context.MemberAccounts
                .Include(m => m.Industry)
                .Where(m => m.IsActive && m.IndustryId == industryId)
                .OrderBy(m => m.FacilityName)
                .ToListAsync();
        }

        public async Task<IEnumerable<MemberAccount>> GetByStatusAsync(MemberAccountStatus status)
        {
            return await _context.MemberAccounts
                .Include(m => m.Industry)
                .Where(m => m.IsActive && m.Status == status)
                .OrderBy(m => m.FacilityName)
                .ToListAsync();
        }

        public async Task<IEnumerable<MemberAccount>> SearchAsync(string searchTerm, int? industryId = null, MemberAccountStatus? status = null, string? w9 = null, string? state = null, int page = 1, int pageSize = 10)
        {
            var query = _context.MemberAccounts
                .Include(m => m.Industry)
                .AsQueryable();

            // Show only active (not soft-deleted) accounts by default
            query = query.Where(m => m.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try exact ID match first
                if (int.TryParse(searchTerm, out int searchId))
                {
                    query = query.Where(m =>
                        m.Id == searchId || // Exact ID match
                        m.MemberNumber == searchTerm || // Exact Member Number match
                        (m.ParentMemberAccountNumber != null && m.ParentMemberAccountNumber == searchTerm) || // Exact Parent Member Number match
                        m.FacilityName.Contains(searchTerm)); // Fuzzy name match
                }
                else
                {
                    // Non-numeric search term
                    query = query.Where(m =>
                        m.MemberNumber == searchTerm || // Exact Member Number match
                        (m.ParentMemberAccountNumber != null && m.ParentMemberAccountNumber == searchTerm) || // Exact Parent Member Number match
                        m.FacilityName.Contains(searchTerm)); // Fuzzy name match
                }
            }

            if (industryId.HasValue)
            {
                query = query.Where(m => m.IndustryId == industryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(w9))
            {
                query = query.Where(m => m.W9 == w9);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(m => m.State == state);
            }

            return await query
                .OrderBy(m => m.FacilityName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string? searchTerm = null, int? industryId = null, MemberAccountStatus? status = null, string? w9 = null, string? state = null)
        {
            var query = _context.MemberAccounts.AsQueryable();

            // Count only active (not soft-deleted)
            query = query.Where(m => m.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int searchId))
                {
                    query = query.Where(m =>
                        m.Id == searchId ||
                        m.MemberNumber == searchTerm ||
                        (m.ParentMemberAccountNumber != null && m.ParentMemberAccountNumber == searchTerm) ||
                        m.FacilityName.Contains(searchTerm));
                }
                else
                {
                    query = query.Where(m =>
                        m.MemberNumber == searchTerm ||
                        (m.ParentMemberAccountNumber != null && m.ParentMemberAccountNumber == searchTerm) ||
                        m.FacilityName.Contains(searchTerm));
                }
            }

            if (industryId.HasValue)
            {
                query = query.Where(m => m.IndustryId == industryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(w9))
            {
                query = query.Where(m => m.W9 == w9);
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(m => m.State == state);
            }

            return await query.CountAsync();
        }

        public override async Task<MemberAccount?> GetByIdAsync(int id)
        {
            return await _context.MemberAccounts
                .Include(m => m.Industry)
                .Include(m => m.CustomerAccounts)
                    .ThenInclude(c => c.Distributor)
                .Include(m => m.CustomerAccounts)
                    .ThenInclude(c => c.OpCo)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<MemberAccount>> GetAllAsync()
        {
            return await _context.MemberAccounts
                .Include(m => m.Industry)
                .Where(m => m.IsActive)
                .OrderBy(m => m.FacilityName)
                .ToListAsync();
        }
    }
}
