using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractDistributorVersionRepository : IContractDistributorVersionRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractDistributorVersionRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractDistributorVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? distributorId = null)
        {
            var q = _context.ContractDistributorsVersion.AsQueryable();
            if (contractId.HasValue) q = q.Where(x => x.ContractId == contractId.Value);
            if (versionNumber.HasValue) q = q.Where(x => x.VersionNumber == versionNumber.Value);
            if (distributorId.HasValue) q = q.Where(x => x.DistributorId == distributorId.Value);
            return await q.OrderBy(x => x.ContractId).ThenBy(x => x.DistributorId).ThenBy(x => x.VersionNumber).ToListAsync();
        }

        public Task<ContractDistributorVersion?> GetByIdAsync(int id) => _context.ContractDistributorsVersion.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ContractDistributorVersion> CreateAsync(ContractDistributorVersion entity)
        {
            _context.ContractDistributorsVersion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractDistributorVersion> UpdateAsync(ContractDistributorVersion entity)
        {
            var existing = await _context.ContractDistributorsVersion.FirstAsync(x => x.Id == entity.Id);
            existing.AssignedBy = entity.AssignedBy;
            existing.AssignedDate = entity.AssignedDate;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractDistributorsVersion.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return false;
            _context.ContractDistributorsVersion.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

