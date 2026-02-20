using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractIndustryVersionRepository : IContractIndustryVersionRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractIndustryVersionRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractIndustryVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? industryId = null)
        {
            var q = _context.ContractIndustriesVersion.AsQueryable();
            if (contractId.HasValue) q = q.Where(x => x.ContractId == contractId.Value);
            if (versionNumber.HasValue) q = q.Where(x => x.VersionNumber == versionNumber.Value);
            if (industryId.HasValue) q = q.Where(x => x.IndustryId == industryId.Value);
            return await q.OrderBy(x => x.ContractId).ThenBy(x => x.IndustryId).ThenBy(x => x.VersionNumber).ToListAsync();
        }

        public Task<ContractIndustryVersion?> GetByIdAsync(int id) => _context.ContractIndustriesVersion.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ContractIndustryVersion> CreateAsync(ContractIndustryVersion entity)
        {
            _context.ContractIndustriesVersion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractIndustryVersion> UpdateAsync(ContractIndustryVersion entity)
        {
            var existing = await _context.ContractIndustriesVersion.FirstAsync(x => x.Id == entity.Id);
            existing.AssignedBy = entity.AssignedBy;
            existing.AssignedDate = entity.AssignedDate;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractIndustriesVersion.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return false;
            _context.ContractIndustriesVersion.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

