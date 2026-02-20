using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Domain.Proposals.Entities;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Services
{
    public class BulkRenewalService : IBulkRenewalService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BulkRenewalService> _logger;

        public BulkRenewalService(ApplicationDbContext context, ILogger<BulkRenewalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get "New Contract" proposal type ID
        private async Task<int> GetNewContractProposalTypeIdAsync()
        {
            var newContractType = await _context.ProposalTypes
                .FirstOrDefaultAsync(pt => pt.Name == "New Contract");

            if (newContractType == null)
            {
                // Create it if it doesn't exist
                newContractType = new ProposalType { Name = "New Contract", IsActive = true };
                _context.ProposalTypes.Add(newContractType);
                await _context.SaveChangesAsync();
            }

            return newContractType.Id;
        }

        // Get "Renewal" proposal type ID
        private async Task<int> GetRenewalProposalTypeIdAsync()
        {
            var renewalType = await _context.ProposalTypes
                .FirstOrDefaultAsync(pt => pt.Name == "Renewal");

            if (renewalType == null)
            {
                // Create it if it doesn't exist
                renewalType = new ProposalType { Name = "Renewal", IsActive = true };
                _context.ProposalTypes.Add(renewalType);
                await _context.SaveChangesAsync();
            }

            return renewalType.Id;
        }

        // Get "Draft" proposal status ID
        private async Task<int> GetDraftProposalStatusIdAsync()
        {
            var draftStatus = await _context.ProposalStatuses
                .FirstOrDefaultAsync(ps => ps.Name == "Draft");

            if (draftStatus == null)
            {
                // Create it if it doesn't exist
                draftStatus = new ProposalStatus { Name = "Draft", IsActive = true };
                _context.ProposalStatuses.Add(draftStatus);
                await _context.SaveChangesAsync();
            }

            return draftStatus.Id;
        }

        public async Task<BulkRenewalResponse> CreateBulkRenewalProposalsAsync(BulkRenewalRequest request)
        {
            var response = new BulkRenewalResponse
            {
                TotalContracts = request.ContractIds.Count
            };

            _logger.LogInformation("Starting bulk renewal for {Count} contracts", request.ContractIds.Count);

            foreach (var contractId in request.ContractIds)
            {
                try
                {
                    var result = await CreateRenewalProposalForContractAsync(contractId, request);
                    response.Results.Add(result);

                    if (result.Success)
                    {
                        response.SuccessfulProposals++;
                        if (result.ProposalId.HasValue)
                        {
                            response.CreatedProposalIds.Add(result.ProposalId.Value);
                        }
                    }
                    else
                    {
                        response.FailedProposals++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating renewal proposal for contract {ContractId}", contractId);
                    response.Results.Add(new ContractRenewalResult
                    {
                        ContractId = contractId,
                        Success = false,
                        ErrorMessage = $"Unexpected error: {ex.Message}"
                    });
                    response.FailedProposals++;
                }
            }

            response.Message = $"Created {response.SuccessfulProposals} of {response.TotalContracts} renewal proposals";
            
            _logger.LogInformation("Bulk renewal completed: {Success} successful, {Failed} failed", 
                response.SuccessfulProposals, response.FailedProposals);

            return response;
        }

        private async Task<ContractRenewalResult> CreateRenewalProposalForContractAsync(
            int contractId,
            BulkRenewalRequest request)
        {
            var result = new ContractRenewalResult { ContractId = contractId };

            // Load contract with all necessary data
            var contract = await _context.Contracts
                .Include(c => c.ContractPrices)
                    .ThenInclude(cp => cp.Product)
                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .Include(c => c.ContractIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null)
            {
                result.Success = false;
                result.ErrorMessage = "Contract not found";
                return result;
            }

            result.ContractNumber = contract.Name;

            // Calculate new term dates
            var newStartDate = contract.EndDate.AddDays(1);
            var contractDuration = (contract.EndDate - contract.StartDate).Days;
            var newEndDate = newStartDate.AddDays(contractDuration);

            // Get manufacturer from ContractManufacturers
            int? manufacturerId = contract.ContractManufacturers?.FirstOrDefault()?.ManufacturerId;

            // If no manufacturer in ContractManufacturers, try to get from first product
            if (!manufacturerId.HasValue && contract.ContractPrices.Any())
            {
                var firstProduct = contract.ContractPrices.FirstOrDefault()?.Product;
                manufacturerId = firstProduct?.ManufacturerId;
            }

            // Validate manufacturer exists
            if (!manufacturerId.HasValue)
            {
                result.Success = false;
                result.ErrorMessage = "Contract has no manufacturer assigned";
                return result;
            }

            // Get proposal type and status IDs
            var renewalTypeId = await GetNewContractProposalTypeIdAsync();
            var draftStatusId = await GetDraftProposalStatusIdAsync();

            // Create proposal
            var proposal = new Proposal
            {
                Title = $"{contract.Name} - New Contract Proposal",
                ProposalTypeId = renewalTypeId,
                ProposalStatusId = draftStatusId,
                ManufacturerId = manufacturerId.Value,
                AmendedContractId = contractId,
                StartDate = newStartDate,
                EndDate = newEndDate,
                InternalNotes = $"Created from contract {contract.Name}. " +
                               (request.ProposalDueDate.HasValue ? $"Due date: {request.ProposalDueDate:yyyy-MM-dd}" : ""),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = request.CreatedBy ?? "System"
            };

            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync();

            result.ProposalId = proposal.Id;

            // Add distributors
            foreach (var contractDistributor in contract.ContractDistributors)
            {
                _context.ProposalDistributors.Add(new ProposalDistributor
                {
                    ProposalId = proposal.Id,
                    DistributorId = contractDistributor.DistributorId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                });
            }

            // Add industries
            foreach (var contractIndustry in contract.ContractIndustries)
            {
                _context.ProposalIndustries.Add(new ProposalIndustry
                {
                    ProposalId = proposal.Id,
                    IndustryId = contractIndustry.IndustryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                });
            }

            // Add op-cos
            foreach (var contractOpCo in contract.ContractOpCos)
            {
                _context.ProposalOpcos.Add(new ProposalOpco
                {
                    ProposalId = proposal.Id,
                    OpCoId = contractOpCo.OpCoId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                });
            }

            await _context.SaveChangesAsync();

            // Add non-discontinued products from contract
            // Get products for the current version
            var activeProducts = contract.ContractPrices
                .Where(cp => cp.Product != null &&
                            cp.Product.Status == ProductStatus.Active &&
                            cp.VersionNumber == contract.CurrentVersionNumber)
                .ToList();

            result.ProductCount = activeProducts.Count;

            foreach (var cp in activeProducts)
            {
                _context.ProposalProducts.Add(new ProposalProduct
                {
                    ProposalId = proposal.Id,
                    ProductId = cp.ProductId,
                    Quantity = (int?)cp.EstimatedQty,
                    // Do NOT copy pricing - manufacturer will enter new pricing
                    Uom = cp.UOM,
                    BillbacksAllowed = cp.BillbacksAllowed,
                    InternalNotes = cp.InternalNotes,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                });
            }

            // Add additional products if specified
            if (request.AdditionalProductIds != null && request.AdditionalProductIds.Any())
            {
                var additionalProducts = await _context.Products
                    .Where(p => request.AdditionalProductIds.Contains(p.Id) && p.Status == ProductStatus.Active)
                    .ToListAsync();

                result.AdditionalProductCount = additionalProducts.Count;

                foreach (var product in additionalProducts)
                {
                    // Skip if product already exists in proposal
                    if (activeProducts.Any(cp => cp.ProductId == product.Id))
                        continue;

                    _context.ProposalProducts.Add(new ProposalProduct
                    {
                        ProposalId = proposal.Id,
                        ProductId = product.Id,
                        Quantity = 0, // To be filled by manufacturer
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = request.CreatedBy ?? "System"
                    });
                }
            }

            await _context.SaveChangesAsync();

            result.Success = true;
            _logger.LogInformation("Created renewal proposal {ProposalId} for contract {ContractId} with {ProductCount} products",
                proposal.Id, contractId, result.ProductCount);

            return result;
        }

        private decimal CalculateAdjustedPrice(decimal currentPrice, int quantity, PricingAdjustment? adjustment)
        {
            if (adjustment == null)
                return currentPrice;

            // Check if quantity threshold applies
            if (adjustment.MinimumQuantityThreshold.HasValue && 
                !adjustment.ApplyToAllProducts &&
                quantity < adjustment.MinimumQuantityThreshold.Value)
            {
                return currentPrice;
            }

            // Apply percentage change
            var adjustmentMultiplier = 1 + (adjustment.PercentageChange / 100);
            return Math.Round(currentPrice * adjustmentMultiplier, 2);
        }

        public async Task<Dictionary<int, string>> ValidateContractsForRenewalAsync(List<int> contractIds)
        {
            var validationResults = new Dictionary<int, string>();

            foreach (var contractId in contractIds)
            {
                var contract = await _context.Contracts
                    .Include(c => c.ContractVersions)
                    .FirstOrDefaultAsync(c => c.Id == contractId);

                if (contract == null)
                {
                    validationResults[contractId] = "Contract not found";
                    continue;
                }

                if (contract.EndDate < DateTime.UtcNow)
                {
                    // Contract already expired - this is actually OK for renewal
                }

                if (!contract.ContractVersions.Any())
                {
                    validationResults[contractId] = "No contract versions found";
                    continue;
                }

                // Contract is valid for renewal
                validationResults[contractId] = string.Empty;
            }

            return validationResults;
        }
    }
}

