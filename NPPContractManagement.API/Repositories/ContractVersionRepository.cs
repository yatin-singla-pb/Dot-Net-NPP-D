using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public class ContractVersionRepository : IContractVersionRepository
    {
        private readonly ApplicationDbContext _context;
        public ContractVersionRepository(ApplicationDbContext context) { _context = context; }

        public async Task<IEnumerable<ContractVersion>> GetVersionsAsync(int contractId)
        {
            // Fetch versions
            var versions = await _context.ContractVersions
                .Where(v => v.ContractId == contractId)
                .OrderByDescending(v => v.VersionNumber)
                .ToListAsync();

            // Base/current pricing from ContractPrices
            var basePrices = await _context.ContractPrices
                .Include(p => p.Product)
                .Where(p => p.ContractId == contractId)
                .ToListAsync();

            var baseProjected = basePrices.Select(cp => new Models.ContractVersionPrice
            {
                ContractId = cp.ContractId,
                PriceId = cp.Id,
                ProductId = cp.ProductId,
                PriceType = cp.PriceType,
                UOM = cp.UOM,
                EstimatedQty = cp.EstimatedQty,
                BillbacksAllowed = cp.BillbacksAllowed,
                PUA = cp.PUA,
                CommercialDelPrice = cp.CommercialDelPrice,
                CommercialFobPrice = cp.CommercialFobPrice,
                CommodityDelPrice = cp.CommodityDelPrice,
                CommodityFobPrice = cp.CommodityFobPrice,
                FFSPrice = cp.FFSPrice,
                NOIPrice = cp.NOIPrice,
                PTV = cp.PTV,
                VersionNumber = cp.VersionNumber,
                Product = cp.Product,
                Price = cp.CommercialDelPrice
                        ?? cp.CommercialFobPrice
                        ?? cp.FFSPrice
                        ?? cp.NOIPrice
                        ?? cp.Allowance,
                Allowance = cp.Allowance
            }).ToList();

            // Historical snapshots from ContractVersionPrice (for older versions)
            var snapshotPrices = await _context.ContractVersionPrices
                .Include(p => p.Product)
                .Where(p => p.ContractId == contractId)
                .ToListAsync();

            var snapshotProjected = snapshotPrices.Select(cp => new Models.ContractVersionPrice
            {
                Id = cp.Id,
                ContractId = cp.ContractId,
                PriceId = cp.PriceId,
                ProductId = cp.ProductId,
                PriceType = cp.PriceType,
                UOM = cp.UOM,
                EstimatedQty = cp.EstimatedQty,
                BillbacksAllowed = cp.BillbacksAllowed,
                PUA = cp.PUA,
                CommercialDelPrice = cp.CommercialDelPrice,
                CommercialFobPrice = cp.CommercialFobPrice,
                CommodityDelPrice = cp.CommodityDelPrice,
                CommodityFobPrice = cp.CommodityFobPrice,
                FFSPrice = cp.FFSPrice,
                NOIPrice = cp.NOIPrice,
                PTV = cp.PTV,
                VersionNumber = cp.VersionNumber,
                Product = cp.Product,
                Price = cp.CommercialDelPrice
                        ?? cp.CommercialFobPrice
                        ?? cp.FFSPrice
                        ?? cp.NOIPrice
                        ?? cp.Allowance,
                Allowance = cp.Allowance
            }).ToList();

            var baseByVersion = baseProjected
                .GroupBy(p => p.VersionNumber)
                .ToDictionary(g => g.Key, g => g.ToList());

            var snapByVersion = snapshotProjected
                .GroupBy(p => p.VersionNumber)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var v in versions)
            {
                if (baseByVersion.TryGetValue(v.VersionNumber, out var curr))
                {
                    v.Prices = curr;
                }
                else if (snapByVersion.TryGetValue(v.VersionNumber, out var hist))
                {
                    v.Prices = hist;
                }
                else
                {
                    v.Prices = new List<Models.ContractVersionPrice>();
                }
            }

            return versions;
        }

        public async Task<ContractVersion?> GetVersionAsync(int contractId, int versionId)
        {
            var version = await _context.ContractVersions
                .FirstOrDefaultAsync(v => v.ContractId == contractId && v.Id == versionId);
            if (version == null) return null;

            // Try base table first for the selected version
            var basePrices = await _context.ContractPrices
                .Include(p => p.Product)
                .Where(p => p.ContractId == contractId && p.VersionNumber == version.VersionNumber)
                .ToListAsync();

            var prices = basePrices.Select(cp => new Models.ContractVersionPrice
            {
                ContractId = cp.ContractId,
                PriceId = cp.Id,
                ProductId = cp.ProductId,
                PriceType = cp.PriceType,
                UOM = cp.UOM,
                EstimatedQty = cp.EstimatedQty,
                BillbacksAllowed = cp.BillbacksAllowed,
                PUA = cp.PUA,
                CommercialDelPrice = cp.CommercialDelPrice,
                CommercialFobPrice = cp.CommercialFobPrice,
                CommodityDelPrice = cp.CommodityDelPrice,
                CommodityFobPrice = cp.CommodityFobPrice,
                FFSPrice = cp.FFSPrice,
                NOIPrice = cp.NOIPrice,
                PTV = cp.PTV,
                VersionNumber = cp.VersionNumber,
                Product = cp.Product,
                Price = cp.CommercialDelPrice
                        ?? cp.CommercialFobPrice
                        ?? cp.FFSPrice
                        ?? cp.NOIPrice
                        ?? cp.Allowance,
                Allowance = cp.Allowance
            }).ToList();

            if (prices.Count == 0)
            {
                // Fallback to snapshot table for historical versions
                var snapshot = await _context.ContractVersionPrices
                    .Include(p => p.Product)
                    .Where(p => p.ContractId == contractId && p.VersionNumber == version.VersionNumber)
                    .ToListAsync();

                prices = snapshot.Select(cp => new Models.ContractVersionPrice
                {
                    Id = cp.Id,
                    ContractId = cp.ContractId,
                    PriceId = cp.PriceId,
                    ProductId = cp.ProductId,
                    PriceType = cp.PriceType,
                    UOM = cp.UOM,
                    EstimatedQty = cp.EstimatedQty,
                    BillbacksAllowed = cp.BillbacksAllowed,
                    PUA = cp.PUA,
                    CommercialDelPrice = cp.CommercialDelPrice,
                    CommercialFobPrice = cp.CommercialFobPrice,
                    CommodityDelPrice = cp.CommodityDelPrice,
                    CommodityFobPrice = cp.CommodityFobPrice,
                    FFSPrice = cp.FFSPrice,
                    NOIPrice = cp.NOIPrice,
                    PTV = cp.PTV,
                    VersionNumber = cp.VersionNumber,
                    Product = cp.Product,
                    Price = cp.CommercialDelPrice
                            ?? cp.CommercialFobPrice
                            ?? cp.FFSPrice
                            ?? cp.NOIPrice
                            ?? cp.Allowance,
                    Allowance = cp.Allowance
                }).ToList();
            }

            version.Prices = prices;
            return version;
        }

        public async Task<ContractVersion> CreateVersionAsync(int contractId, ContractVersion version, IEnumerable<ContractVersionPrice> prices, string createdBy)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // compute next VersionNumber safely
                var maxVersion = await _context.ContractVersions
                    .Where(v => v.ContractId == contractId)
                    .MaxAsync(v => (int?)v.VersionNumber) ?? 0;
                version.ContractId = contractId;
                version.VersionNumber = maxVersion + 1;
                version.AssignedDate = DateTime.UtcNow;
                version.AssignedBy = createdBy;

                _context.ContractVersions.Add(version);
                await _context.SaveChangesAsync();

                // Note: Version-level price rows are stored in ContractVersionPrice and require a base ContractPrice (PriceId) FK.
                // The supplied 'prices' here are for UI draft purposes only and are not persisted directly.
                // If needed later, they should be materialized via ContractPrice and then mirrored to ContractVersionPrice.

                await tx.CommitAsync();
                return version;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<ContractVersion> UpdateVersionAsync(ContractVersion version, string modifiedBy)
        {
            var existing = await _context.ContractVersions.FirstAsync(v => v.Id == version.Id);
            existing.Name = version.Name;
            existing.ForeignContractId = version.ForeignContractId;
            existing.SendToPerformance = version.SendToPerformance;
            existing.IsSuspended = version.IsSuspended;
            existing.SuspendedDate = version.SuspendedDate;
            existing.InternalNotes = version.InternalNotes;
            existing.StartDate = version.StartDate;
            existing.EndDate = version.EndDate;
            existing.AssignedDate = DateTime.UtcNow;
            existing.AssignedBy = modifiedBy;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}

