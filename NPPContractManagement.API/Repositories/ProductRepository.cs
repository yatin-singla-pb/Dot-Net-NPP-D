using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetBySKUAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.DistributorProductCodes)
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<Product?> GetByGTINAsync(string gtin)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.DistributorProductCodes)
                .FirstOrDefaultAsync(p => p.GTIN == gtin);
        }

        public async Task<Product?> GetByUPCAsync(string upc)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.DistributorProductCodes)
                .FirstOrDefaultAsync(p => p.UPC == upc);
        }

        public async Task<Product?> GetByManufacturerProductCodeAsync(string manufacturerProductCode)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.DistributorProductCodes)
                .FirstOrDefaultAsync(p => p.ManufacturerProductCode == manufacturerProductCode);
        }

        public async Task<bool> ExistsBySKUAsync(string sku, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.SKU == sku);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByGTINAsync(string gtin, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.GTIN == gtin);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByUPCAsync(string upc, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.UPC == upc);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Product>> GetByManufacturerIdAsync(int manufacturerId)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Where(p => p.ManufacturerId == manufacturerId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Where(p => p.Status == status)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Where(p => p.Category == category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, int? manufacturerId = null, ProductStatus? status = null, string? category = null, string? brand = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Manufacturer)
                .AsQueryable();

            // Only active records
            query = query.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Fuzzy name, fuzzy brand, exact code match, fuzzy GTIN
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) || // Fuzzy name
                    (p.Brand != null && p.Brand.Contains(searchTerm)) || // Fuzzy brand
                    (p.ManufacturerProductCode != null && p.ManufacturerProductCode == searchTerm) || // Exact code match
                    (p.GTIN != null && p.GTIN.Contains(searchTerm))); // Fuzzy GTIN
            }

            if (manufacturerId.HasValue)
            {
                query = query.Where(p => p.ManufacturerId == manufacturerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            return await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(int? manufacturerId = null, ProductStatus? status = null, string? category = null, string? brand = null)
        {
            var query = _context.Products.AsQueryable();

            // Only active records
            query = query.Where(p => p.IsActive);

            if (manufacturerId.HasValue)
            {
                query = query.Where(p => p.ManufacturerId == manufacturerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Where(p => p.Status == ProductStatus.Active && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.DistributorProductCodes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Manufacturer)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByManufacturerIdsAsync(IEnumerable<int> manufacturerIds)
        {
            var ids = (manufacturerIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (ids.Count == 0) return new List<Product>();
            return await _context.Products
                .Include(p => p.Manufacturer)
                .Where(p => ids.Contains(p.ManufacturerId))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctBrandsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive && !string.IsNullOrWhiteSpace(p.Brand))
                .Select(p => p.Brand!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

    }
}
