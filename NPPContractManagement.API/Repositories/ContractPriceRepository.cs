using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractPriceRepository : IContractPriceRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractPriceRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractPrice>> GetAllAsync(int? productId = null, int? versionNumber = null)
        {
            var query = _context.ContractPrices
                .Include(cp => cp.Product)
                .Include(cp => cp.Contract)
                .AsQueryable();
            if (productId.HasValue) query = query.Where(cp => cp.ProductId == productId.Value);
            if (versionNumber.HasValue) query = query.Where(cp => cp.VersionNumber == versionNumber.Value);
            return await query.OrderBy(cp => cp.ProductId).ThenBy(cp => cp.VersionNumber).ToListAsync();
        }

        public async Task<ContractPrice?> GetByIdAsync(int id)
        {
            return await _context.ContractPrices
                .Include(cp => cp.Product)
                .Include(cp => cp.Contract)
                .FirstOrDefaultAsync(cp => cp.Id == id);
        }

        public async Task<ContractPrice> CreateAsync(ContractPrice entity)
        {
            _context.ContractPrices.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ContractPrice> UpdateAsync(ContractPrice entity)
        {
            var existing = await _context.ContractPrices.FirstAsync(cp => cp.Id == entity.Id);
            existing.PriceType = entity.PriceType;
            existing.Allowance = entity.Allowance;
            existing.CommercialDelPrice = entity.CommercialDelPrice;
            existing.CommercialFobPrice = entity.CommercialFobPrice;
            existing.CommodityDelPrice = entity.CommodityDelPrice;
            existing.CommodityFobPrice = entity.CommodityFobPrice;
            existing.UOM = entity.UOM;
            existing.EstimatedQty = entity.EstimatedQty;
            existing.BillbacksAllowed = entity.BillbacksAllowed;
            existing.PUA = entity.PUA;
            existing.FFSPrice = entity.FFSPrice;
            existing.NOIPrice = entity.NOIPrice;
            existing.PTV = entity.PTV;
            existing.InternalNotes = entity.InternalNotes;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = entity.ModifiedBy;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContractPrices.FirstOrDefaultAsync(cp => cp.Id == id);
            if (existing == null) return false;
            _context.ContractPrices.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

