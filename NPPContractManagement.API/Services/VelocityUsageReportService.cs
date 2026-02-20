using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.DTOs.Proposals;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using System.Text.Json;

namespace NPPContractManagement.API.Services
{
    public class VelocityUsageReportService : IVelocityUsageReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVelocityRepository _velocityRepository;
        private readonly IProposalService _proposalService;

        public VelocityUsageReportService(
            ApplicationDbContext context,
            IVelocityRepository velocityRepository,
            IProposalService proposalService)
        {
            _context = context;
            _velocityRepository = velocityRepository;
            _proposalService = proposalService;
        }

        public async Task<VelocityUsageReportResponse> GenerateReportAsync(VelocityUsageReportRequest request)
        {
            // Get velocity shipments with filters
            (List<VelocityShipment> shipments, int totalCount) = await _velocityRepository.GetVelocityUsageDataAsync(
                request.StartDate,
                request.EndDate,
                request.Keyword,
                request.ManufacturerIds,
                request.OpCoIds,
                request.IndustryIds,
                1, // Get all for aggregation
                int.MaxValue, // Get all for aggregation
                null,
                null);

            // Parse and aggregate the data
            var aggregatedData = AggregateVelocityData(shipments);

            // Apply pagination to aggregated results
            var pagedData = aggregatedData
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new VelocityUsageReportResponse
            {
                Data = pagedData,
                TotalCount = aggregatedData.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        private List<VelocityUsageAggregateDto> AggregateVelocityData(List<VelocityShipment> shipments)
        {
            var grouped = new Dictionary<string, VelocityUsageAggregateDto>();

            foreach (var shipment in shipments)
            {
                if (string.IsNullOrEmpty(shipment.ManifestLine))
                    continue;

                try
                {
                    var manifestData = JsonSerializer.Deserialize<VelocityShipmentCsvRow>(shipment.ManifestLine);
                    if (manifestData == null)
                        continue;

                    // Create a unique key for grouping (product + manufacturer)
                    var groupKey = $"{manifestData.ProductNumber}|{manifestData.ManufacturerName}|{manifestData.GTIN}";

                    if (!grouped.ContainsKey(groupKey))
                    {
                        grouped[groupKey] = new VelocityUsageAggregateDto
                        {
                            GroupKey = groupKey,
                            Manufacturer = manifestData.ManufacturerName,
                            Product = manifestData.Description,
                            ProductId = manifestData.ProductNumber,
                            DistributorProductCode = manifestData.ProductNumber,
                            Brand = manifestData.Brand,
                            PackSize = manifestData.PackSize,
                            ProductDescription = manifestData.Description,
                            MfrProductCode = manifestData.CorpManufNumber,
                            GTIN = manifestData.GTIN,
                            CasesPurchased = 0,
                            MinShipmentDate = shipment.ShippedAt,
                            MaxShipmentDate = shipment.ShippedAt,
                            AvgLandedCost = 0
                        };
                    }

                    var aggregate = grouped[groupKey];

                    // Update aggregates
                    if (int.TryParse(manifestData.Qty, out int qty))
                    {
                        aggregate.CasesPurchased += qty;
                    }

                    if (shipment.ShippedAt.HasValue)
                    {
                        if (!aggregate.MinShipmentDate.HasValue || shipment.ShippedAt < aggregate.MinShipmentDate)
                            aggregate.MinShipmentDate = shipment.ShippedAt;

                        if (!aggregate.MaxShipmentDate.HasValue || shipment.ShippedAt > aggregate.MaxShipmentDate)
                            aggregate.MaxShipmentDate = shipment.ShippedAt;
                    }
                }
                catch (JsonException)
                {
                    // Skip invalid JSON
                    continue;
                }
            }

            // Calculate average landed cost
            foreach (var kvp in grouped)
            {
                var groupKey = kvp.Key;
                var aggregate = kvp.Value;

                var relevantShipments = shipments.Where(s =>
                {
                    if (string.IsNullOrEmpty(s.ManifestLine))
                        return false;

                    try
                    {
                        var manifestData = JsonSerializer.Deserialize<VelocityShipmentCsvRow>(s.ManifestLine);
                        var key = $"{manifestData?.ProductNumber}|{manifestData?.ManufacturerName}|{manifestData?.GTIN}";
                        return key == groupKey;
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();

                var landedCosts = new List<decimal>();
                foreach (var shipment in relevantShipments)
                {
                    try
                    {
                        var manifestData = JsonSerializer.Deserialize<VelocityShipmentCsvRow>(shipment.ManifestLine!);
                        if (manifestData != null && decimal.TryParse(manifestData.LandedCost, out decimal cost))
                        {
                            landedCosts.Add(cost);
                        }
                    }
                    catch { }
                }

                if (landedCosts.Any())
                {
                    aggregate.AvgLandedCost = landedCosts.Average();
                }
            }

            return grouped.Values.OrderByDescending(a => a.CasesPurchased).ToList();
        }

        public async Task<List<VelocityUsageDetailDto>> GetDetailRecordsAsync(string groupKey, VelocityUsageReportRequest request)
        {
            // Parse group key
            var parts = groupKey.Split('|');
            if (parts.Length != 3)
                return new List<VelocityUsageDetailDto>();

            var productNumber = parts[0];
            var manufacturerName = parts[1];
            var gtin = parts[2];

            // Get all shipments for this group
            (List<VelocityShipment> shipments, int _) = await _velocityRepository.GetVelocityUsageDataAsync(
                request.StartDate,
                request.EndDate,
                null,
                request.ManufacturerIds,
                request.OpCoIds,
                request.IndustryIds,
                1,
                int.MaxValue,
                null,
                null);

            var details = new List<VelocityUsageDetailDto>();

            foreach (var shipment in shipments)
            {
                if (string.IsNullOrEmpty(shipment.ManifestLine))
                    continue;

                try
                {
                    var manifestData = JsonSerializer.Deserialize<VelocityShipmentCsvRow>(shipment.ManifestLine);
                    if (manifestData == null)
                        continue;

                    // Check if this shipment belongs to the group
                    if (manifestData.ProductNumber == productNumber &&
                        manifestData.ManufacturerName == manufacturerName &&
                        manifestData.GTIN == gtin)
                    {
                        details.Add(new VelocityUsageDetailDto
                        {
                            Id = shipment.Id,
                            ProductId = manifestData.ProductNumber,
                            DistributorProductCode = manifestData.ProductNumber,
                            Brand = manifestData.Brand,
                            PackSize = manifestData.PackSize,
                            ProductDescription = manifestData.Description,
                            MfrProductCode = manifestData.CorpManufNumber,
                            GTIN = manifestData.GTIN,
                            Manufacturer = manifestData.ManufacturerName,
                            CasesPurchased = int.TryParse(manifestData.Qty, out int qty) ? qty : null,
                            InvoiceDate = DateTime.TryParse(manifestData.InvoiceDate, out DateTime invDate) ? invDate : null,
                            InvoiceNumber = manifestData.InvoiceNumber,
                            LandedCost = decimal.TryParse(manifestData.LandedCost, out decimal cost) ? cost : null,
                            OpCo = manifestData.OpCo,
                            CustomerNumber = manifestData.CustomerNumber,
                            CustomerName = manifestData.CustomerName
                        });
                    }
                }
                catch (JsonException)
                {
                    continue;
                }
            }

            return details;
        }

        public async Task<Dictionary<string, List<int>>> CheckExistingContractsAsync(List<string> groupKeys)
        {
            var result = new Dictionary<string, List<int>>();

            foreach (var groupKey in groupKeys)
            {
                var parts = groupKey.Split('|');
                if (parts.Length != 3)
                    continue;

                var productNumber = parts[0];
                var manufacturerName = parts[1];

                // Find contracts that have this product and manufacturer
                var contracts = await _context.Contracts
                    .Include(c => c.ContractManufacturers)
                        .ThenInclude(cm => cm.Manufacturer)
                    .Include(c => c.ContractProducts)
                        .ThenInclude(cp => cp.Product)
                    .Where(c => c.Status == ContractStatus.Active &&
                                c.ContractManufacturers.Any(cm => cm.Manufacturer != null && cm.Manufacturer.Name == manufacturerName))
                    .ToListAsync();

                // Filter contracts that have matching products
                var matchingContractIds = contracts
                    .Where(c => c.ContractProducts.Any(cp => cp.Product != null && cp.Product.ManufacturerProductCode == productNumber))
                    .Select(c => c.Id)
                    .ToList();

                result[groupKey] = matchingContractIds;
            }

            return result;
        }

        public async Task<List<int>> CreateProposalsFromVelocityAsync(CreateProposalFromVelocityRequest request, string createdBy)
        {
            var proposalIds = new List<int>();

            // Group selected items by manufacturer
            var groupedByManufacturer = new Dictionary<string, List<string>>();

            foreach (var groupKey in request.SelectedGroupKeys)
            {
                var parts = groupKey.Split('|');
                if (parts.Length != 3)
                    continue;

                var manufacturerName = parts[1];

                if (!groupedByManufacturer.ContainsKey(manufacturerName))
                {
                    groupedByManufacturer[manufacturerName] = new List<string>();
                }

                groupedByManufacturer[manufacturerName].Add(groupKey);
            }

            // Create one proposal per manufacturer
            foreach (var kvp in groupedByManufacturer)
            {
                var manufacturerName = kvp.Key;
                var groupKeys = kvp.Value;

                // Find manufacturer ID
                var manufacturer = await _context.Manufacturers
                    .FirstOrDefaultAsync(m => m.Name == manufacturerName);

                if (manufacturer == null)
                    continue;

                // Check if contracts exist for these products
                var contractCheck = await CheckExistingContractsAsync(groupKeys);
                var hasExistingContracts = contractCheck.Values.Any(list => list.Any());

                // Create proposal DTO
                var proposalDto = new ProposalCreateDto
                {
                    Title = request.ProposalTitle ?? $"Velocity Usage Proposal - {manufacturerName}",
                    ProposalTypeId = 1, // Default type
                    ProposalStatusId = 1, // Requested
                    ManufacturerId = manufacturer.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = request.ProposalDueDate ?? DateTime.UtcNow.AddYears(1),
                    Products = new List<ProposalProductCreateDto>()
                };

                // Add products from velocity data
                foreach (var groupKey in groupKeys)
                {
                    var details = await GetDetailRecordsAsync(groupKey, new VelocityUsageReportRequest
                    {
                        StartDate = DateTime.UtcNow.AddDays(-30),
                        EndDate = DateTime.UtcNow
                    });

                    if (!details.Any())
                        continue;

                    var firstDetail = details.First();
                    var totalQuantity = details.Sum(d => d.CasesPurchased ?? 0);

                    // Apply quantity adjustment if specified
                    if (request.QuantityAdjustmentPercent.HasValue && request.MinimumQuantityThreshold.HasValue)
                    {
                        if (totalQuantity >= request.MinimumQuantityThreshold.Value)
                        {
                            totalQuantity = (int)(totalQuantity * (1 + request.QuantityAdjustmentPercent.Value / 100));
                        }
                    }

                    // Find product in database
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.ManufacturerProductCode == firstDetail.ProductId);

                    if (product != null)
                    {
                        proposalDto.Products.Add(new ProposalProductCreateDto
                        {
                            ProductId = product.Id,
                            Quantity = totalQuantity
                        });
                    }
                }

                // Create the proposal
                var createdProposal = await _proposalService.CreateProposalAsync(proposalDto, createdBy);
                proposalIds.Add(createdProposal.Id);
            }

            return proposalIds;
        }
    }
}

