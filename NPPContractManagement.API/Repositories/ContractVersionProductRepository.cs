using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractVersionProductRepository : IContractVersionProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractVersionProductRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractVersionProduct>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? productId = null)
        {
            var q = _context.ContractVersionProducts.AsQueryable();
            if (contractId.HasValue) q = q.Where(x => x.ContractId == contractId.Value);
            if (versionNumber.HasValue) q = q.Where(x => x.VersionNumber == versionNumber.Value);
            if (productId.HasValue) q = q.Where(x => x.ProductId == productId.Value);
            return await q.OrderBy(x => x.ContractId).ThenBy(x => x.ProductId).ThenBy(x => x.VersionNumber).ToListAsync();
        }

        public Task<ContractVersionProduct?> GetByIdAsync(int id) => _context.ContractVersionProducts.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ContractVersionProduct> CreateAsync(ContractVersionProduct entity)
        {
            _context.ContractVersionProducts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractVersionProduct> UpdateAsync(ContractVersionProduct entity)
        {
            var existing = await _context.ContractVersionProducts.FirstAsync(x => x.Id == entity.Id);
            existing.AssignedBy = entity.AssignedBy;
            existing.AssignedDate = entity.AssignedDate;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractVersionProducts.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return false;
            _context.ContractVersionProducts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

