using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public class ConflictDetectionService : IConflictDetectionService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ConflictDetectionService> _logger;

        public ConflictDetectionService(ApplicationDbContext db, ILogger<ConflictDetectionService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ProposalConflictResultDto> DetectConflictsAsync(int proposalId)
        {
            var proposal = await _db.Proposals
                .Include(p => p.Products)
                .Include(p => p.Opcos)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException($"Proposal {proposalId} not found", nameof(proposalId));

            // If dates are missing, no conflicts possible
            if (proposal.StartDate == null || proposal.EndDate == null)
                return new ProposalConflictResultDto { ProposalId = proposalId };

            var proposalStart = proposal.StartDate.Value;
            var proposalEnd = proposal.EndDate.Value;

            var proposalProductIds = proposal.Products
                .Where(p => p.IsActive)
                .Select(p => p.ProductId)
                .Distinct()
                .ToList();

            if (!proposalProductIds.Any())
                return new ProposalConflictResultDto { ProposalId = proposalId };

            var proposalOpCoIds = proposal.Opcos
                .Where(o => o.IsActive)
                .Select(o => o.OpCoId)
                .Distinct()
                .ToList();

            bool proposalIsNationwide = !proposalOpCoIds.Any();
            var amendedContractId = proposal.AmendedContractId;

            _logger.LogInformation(
                "Detecting conflicts for proposal {ProposalId}: {ProductCount} products, {OpCoCount} OpCos, dates {Start}-{End}, amended={AmendedId}",
                proposalId, proposalProductIds.Count, proposalOpCoIds.Count, proposalStart, proposalEnd, amendedContractId);

            // Single optimized query: find all contract prices where
            // - ProductId matches one of the proposal's products
            // - Contract is NOT suspended
            // - Date ranges overlap
            // - Contract is NOT the amended source contract
            var query = _db.ContractPrices
                .Include(cp => cp.Contract)
                    .ThenInclude(c => c.ContractOpCos)
                        .ThenInclude(co => co.OpCo)
                .Include(cp => cp.Contract)
                    .ThenInclude(c => c.ContractManufacturers)
                        .ThenInclude(cm => cm.Manufacturer)
                .Include(cp => cp.Product)
                .Where(cp =>
                    proposalProductIds.Contains(cp.ProductId)
                    && !cp.Contract.IsSuspended
                    && cp.Contract.StartDate <= proposalEnd
                    && cp.Contract.EndDate >= proposalStart
                );

            // Exclude the amended source contract (for amendments)
            if (amendedContractId.HasValue)
            {
                query = query.Where(cp => cp.ContractId != amendedContractId.Value);
            }

            var conflictingPrices = await query.AsNoTracking().ToListAsync();

            _logger.LogInformation(
                "Found {Count} potential conflicting contract prices for proposal {ProposalId}",
                conflictingPrices.Count, proposalId);

            // In-memory: filter by OpCo overlap and build conflict DTOs
            var conflicts = new List<ProductConflictDto>();

            foreach (var cp in conflictingPrices)
            {
                var contractOpCoIds = cp.Contract.ContractOpCos
                    .Where(co => co.IsActive)
                    .Select(co => co.OpCoId)
                    .ToList();

                bool contractIsNationwide = !contractOpCoIds.Any();

                List<OpCoConflictDto> overlappingOpCos;
                bool isNationwideConflict = false;

                if (proposalIsNationwide || contractIsNationwide)
                {
                    // Either side is nationwide → conflict
                    isNationwideConflict = true;

                    if (proposalIsNationwide && contractIsNationwide)
                    {
                        // Both nationwide
                        overlappingOpCos = new List<OpCoConflictDto>();
                    }
                    else if (contractIsNationwide)
                    {
                        // Contract is nationwide, show proposal's OpCos as affected
                        overlappingOpCos = proposal.Opcos
                            .Where(o => o.IsActive)
                            .Select(o => new OpCoConflictDto
                            {
                                OpCoId = o.OpCoId,
                                OpCoName = o.OpCo?.Name ?? $"OpCo #{o.OpCoId}"
                            }).ToList();
                    }
                    else
                    {
                        // Proposal is nationwide, show contract's OpCos as affected
                        overlappingOpCos = cp.Contract.ContractOpCos
                            .Where(co => co.IsActive)
                            .Select(co => new OpCoConflictDto
                            {
                                OpCoId = co.OpCoId,
                                OpCoName = co.OpCo?.Name ?? $"OpCo #{co.OpCoId}"
                            }).ToList();
                    }
                }
                else
                {
                    // Both have specific OpCos - find intersection
                    var intersection = proposalOpCoIds
                        .Intersect(contractOpCoIds)
                        .ToHashSet();

                    if (!intersection.Any())
                        continue; // No OpCo overlap → not a conflict

                    overlappingOpCos = cp.Contract.ContractOpCos
                        .Where(co => co.IsActive && intersection.Contains(co.OpCoId))
                        .Select(co => new OpCoConflictDto
                        {
                            OpCoId = co.OpCoId,
                            OpCoName = co.OpCo?.Name ?? $"OpCo #{co.OpCoId}"
                        }).ToList();
                }

                // Compute overlap period
                var overlapStart = proposalStart > cp.Contract.StartDate ? proposalStart : cp.Contract.StartDate;
                var overlapEnd = proposalEnd < cp.Contract.EndDate ? proposalEnd : cp.Contract.EndDate;

                var manufacturerName = cp.Contract.ContractManufacturers
                    .FirstOrDefault()?.Manufacturer?.Name;

                conflicts.Add(new ProductConflictDto
                {
                    ProductId = cp.ProductId,
                    ProductName = cp.Product?.Name ?? string.Empty,
                    ManufacturerProductCode = cp.Product?.ManufacturerProductCode,
                    ConflictingContractId = cp.ContractId,
                    ConflictingContractName = cp.Contract.Name,
                    ConflictingContractVersionNumber = cp.Contract.CurrentVersionNumber,
                    ConflictingContractForeignId = cp.Contract.ForeignContractId,
                    ConflictingManufacturerName = manufacturerName,
                    OverlappingOpCos = overlappingOpCos,
                    IsNationwideConflict = isNationwideConflict,
                    ProposalStartDate = proposalStart,
                    ProposalEndDate = proposalEnd,
                    ContractStartDate = cp.Contract.StartDate,
                    ContractEndDate = cp.Contract.EndDate,
                    OverlapStartDate = overlapStart,
                    OverlapEndDate = overlapEnd,
                });
            }

            _logger.LogInformation(
                "Conflict detection complete for proposal {ProposalId}: {ConflictCount} conflicts found",
                proposalId, conflicts.Count);

            return new ProposalConflictResultDto
            {
                ProposalId = proposalId,
                HasConflicts = conflicts.Any(),
                TotalConflictCount = conflicts.Count,
                Conflicts = conflicts
            };
        }
    }
}
