using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class DistributorProductCodeRepository : Repository<DistributorProductCode>, IDistributorProductCodeRepository
    {
        public DistributorProductCodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<DistributorProductCode?> GetByIdAsync(int id)
        {
            return await _context.DistributorProductCodes
                .Include(x => x.Distributor)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<DistributorProductCode>> SearchAsync(
            string? searchTerm,
            IEnumerable<int>? distributorIds = null,
            IEnumerable<int>? productIds = null,
            int? productStatus = null,
            int page = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc")
        {
            var query = _context.DistributorProductCodes
                .Include(x => x.Distributor)
                .Include(x => x.Product)
                    .ThenInclude(p => p.Manufacturer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(x => x.Id == searchId);
                }
                else
                {
                    // Fuzzy matches: product description, product code, distributor code
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(x =>
                        (x.Product.Description != null && x.Product.Description.ToLower().Contains(lowerTerm)) ||
                        (x.Product.ManufacturerProductCode != null && x.Product.ManufacturerProductCode.ToLower().Contains(lowerTerm)) ||
                        x.DistributorCode.ToLower().Contains(lowerTerm));
                }
            }

            if (distributorIds != null && distributorIds.Any())
            {
                query = query.Where(x => distributorIds.Contains(x.DistributorId));
            }

            if (productIds != null && productIds.Any())
            {
                query = query.Where(x => productIds.Contains(x.ProductId));
            }

            if (productStatus.HasValue)
            {
                query = query.Where(x => x.Product.Status == (ProductStatus)productStatus.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var dirDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
                query = sortBy switch
                {
                    nameof(DistributorProductCode.DistributorCode) => dirDesc ? query.OrderByDescending(x => x.DistributorCode) : query.OrderBy(x => x.DistributorCode),
                    nameof(DistributorProductCode.DistributorId) => dirDesc ? query.OrderByDescending(x => x.DistributorId) : query.OrderBy(x => x.DistributorId),
                    nameof(DistributorProductCode.ProductId) => dirDesc ? query.OrderByDescending(x => x.ProductId) : query.OrderBy(x => x.ProductId),
                    _ => dirDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
                };
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(string? searchTerm, IEnumerable<int>? distributorIds = null, IEnumerable<int>? productIds = null, int? productStatus = null)
        {
            var query = _context.DistributorProductCodes
                .Include(x => x.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse as integer for exact ID match
                if (int.TryParse(searchTerm.Trim(), out int searchId))
                {
                    query = query.Where(x => x.Id == searchId);
                }
                else
                {
                    // Fuzzy matches: product description, product code, distributor code
                    var lowerTerm = searchTerm.Trim().ToLower();
                    query = query.Where(x =>
                        (x.Product.Description != null && x.Product.Description.ToLower().Contains(lowerTerm)) ||
                        (x.Product.ManufacturerProductCode != null && x.Product.ManufacturerProductCode.ToLower().Contains(lowerTerm)) ||
                        x.DistributorCode.ToLower().Contains(lowerTerm));
                }
            }

            if (distributorIds != null && distributorIds.Any())
            {
                query = query.Where(x => distributorIds.Contains(x.DistributorId));
            }

            if (productIds != null && productIds.Any())
            {
                query = query.Where(x => productIds.Contains(x.ProductId));
            }

            if (productStatus.HasValue)
            {
                query = query.Where(x => x.Product.Status == (ProductStatus)productStatus.Value);
            }

            return await query.CountAsync();
        }

        public async Task<DistributorProductCode?> FindByDistributorAndCodeAsync(int distributorId, string distributorCode, int? excludeId = null)
        {
            var code = distributorCode.Trim();
            var q = _context.DistributorProductCodes.Where(x => x.DistributorId == distributorId && x.DistributorCode == code);
            if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
            return await q.FirstOrDefaultAsync();
        }

        public async Task<DistributorProductCode?> FindByDistributorAndProductAsync(int distributorId, int productId, int? excludeId = null)
        {
            var q = _context.DistributorProductCodes.Where(x => x.DistributorId == distributorId && x.ProductId == productId);
            if (excludeId.HasValue) q = q.Where(x => x.Id != excludeId.Value);
            return await q.FirstOrDefaultAsync();
        }
    }
}

