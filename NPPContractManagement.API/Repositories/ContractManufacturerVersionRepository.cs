using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractManufacturerVersionRepository : IContractManufacturerVersionRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractManufacturerVersionRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractManufacturerVersion>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? manufacturerId = null)
        {
            var q = _context.ContractManufacturersVersion.AsQueryable();
            if (contractId.HasValue) q = q.Where(x => x.ContractId == contractId.Value);
            if (versionNumber.HasValue) q = q.Where(x => x.VersionNumber == versionNumber.Value);
            if (manufacturerId.HasValue) q = q.Where(x => x.ManufacturerId == manufacturerId.Value);
            return await q.OrderBy(x => x.ContractId).ThenBy(x => x.ManufacturerId).ThenBy(x => x.VersionNumber).ToListAsync();
        }

        public Task<ContractManufacturerVersion?> GetByIdAsync(int id) => _context.ContractManufacturersVersion.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ContractManufacturerVersion> CreateAsync(ContractManufacturerVersion entity)
        {
            _context.ContractManufacturersVersion.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractManufacturerVersion> UpdateAsync(ContractManufacturerVersion entity)
        {
            var existing = await _context.ContractManufacturersVersion.FirstAsync(x => x.Id == entity.Id);
            existing.AssignedBy = entity.AssignedBy;
            existing.AssignedDate = entity.AssignedDate;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractManufacturersVersion.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return false;
            _context.ContractManufacturersVersion.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

