using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractOpCoVersionRepository : IContractOpCoVersionRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractOpCoVersionRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractOpCoVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? opCoId = null)
        {
            var q = _context.ContractOpCosVersion.AsQueryable();
            if (contractId.HasValue) q = q.Where(x => x.ContractId == contractId.Value);
            if (versionNumber.HasValue) q = q.Where(x => x.VersionNumber == versionNumber.Value);
            if (opCoId.HasValue) q = q.Where(x => x.OpCoId == opCoId.Value);
            return await q.OrderBy(x => x.ContractId).ThenBy(x => x.OpCoId).ThenBy(x => x.VersionNumber).ToListAsync();
        }

        public Task<ContractOpCoVersion?> GetByIdAsync(int id) => _context.ContractOpCosVersion.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ContractOpCoVersion> CreateAsync(ContractOpCoVersion entity)
        {
            _context.ContractOpCosVersion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractOpCoVersion> UpdateAsync(ContractOpCoVersion entity)
        {
            var existing = await _context.ContractOpCosVersion.FirstAsync(x => x.Id == entity.Id);
            existing.AssignedBy = entity.AssignedBy;
            existing.AssignedDate = entity.AssignedDate;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractOpCosVersion.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return false;
            _context.ContractOpCosVersion.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

