using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs.Reports;

namespace NPPContractManagement.API.Services
{
    public interface IContractPricingReportService
    {
        Task<ContractPricingReportResponse> GenerateReportAsync(ContractPricingReportRequest request, IEnumerable<int>? allowedManufacturerIds = null);
    }

    public class ContractPricingReportService : IContractPricingReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContractPricingReportService> _logger;

        public ContractPricingReportService(
            ApplicationDbContext context,
            ILogger<ContractPricingReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ContractPricingReportResponse> GenerateReportAsync(
            ContractPricingReportRequest request,
            IEnumerable<int>? allowedManufacturerIds = null)
        {
            var allRows = new List<ContractPricingReportRow>();

            // Get all contracts with their relationships
            var contractsQuery = _context.Contracts
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .Include(c => c.ContractOpCos)
                    .ThenInclude(co => co.OpCo)
                .Include(c => c.ContractIndustries)
                    .ThenInclude(ci => ci.Industry)
                .Include(c => c.ContractDistributors)
                    .ThenInclude(cd => cd.Distributor)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.ContractNumber))
            {
                contractsQuery = contractsQuery.Where(c => c.ForeignContractId != null &&
                    c.ForeignContractId.Contains(request.ContractNumber));
            }

            if (!string.IsNullOrWhiteSpace(request.ContractName))
            {
                contractsQuery = contractsQuery.Where(c => c.Name.Contains(request.ContractName));
            }

            if (request.ManufacturerId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c =>
                    c.ContractManufacturers.Any(cm => cm.ManufacturerId == request.ManufacturerId.Value));
            }

            if (request.OpCoId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c =>
                    c.ContractOpCos.Any(co => co.OpCoId == request.OpCoId.Value));
            }

            if (request.IndustryId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c =>
                    c.ContractIndustries.Any(ci => ci.IndustryId == request.IndustryId.Value));
            }

            if (request.DistributorId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c =>
                    c.ContractDistributors.Any(cd => cd.DistributorId == request.DistributorId.Value));
            }

            if (request.StartDateFrom.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.StartDate >= request.StartDateFrom.Value);
            }

            if (request.StartDateTo.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.StartDate <= request.StartDateTo.Value);
            }

            if (request.EndDateFrom.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.EndDate >= request.EndDateFrom.Value);
            }

            if (request.EndDateTo.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.EndDate <= request.EndDateTo.Value);
            }

            // Apply manufacturer restriction for manufacturer users
            if (allowedManufacturerIds != null && allowedManufacturerIds.Any())
            {
                var allowedIds = allowedManufacturerIds.ToList();
                contractsQuery = contractsQuery.Where(c =>
                    c.ContractManufacturers.Any(cm => allowedIds.Contains(cm.ManufacturerId)));
            }

            var contracts = await contractsQuery.ToListAsync();

            foreach (var contract in contracts)
            {
                // Get manufacturer name
                var manufacturerName = contract.ContractManufacturers?.FirstOrDefault()?.Manufacturer?.Name ?? "";

                // Get op-cos as comma-separated string
                var opCos = string.Join(", ", contract.ContractOpCos?.Select(co => co.OpCo?.Name ?? "") ?? Array.Empty<string>());

                // Get industry
                var industry = contract.ContractIndustries?.FirstOrDefault()?.Industry?.Name;

                // Get all contract prices (current version)
                var contractPrices = await _context.ContractPrices
                    .Include(cp => cp.Product)
                    .Where(cp => cp.ContractId == contract.Id)
                    .ToListAsync();

                // Filter by product if specified
                if (request.ProductId.HasValue)
                {
                    contractPrices = contractPrices.Where(cp => cp.ProductId == request.ProductId.Value).ToList();
                }

                // Add rows for current contract prices
                foreach (var price in contractPrices)
                {
                    allRows.Add(CreateReportRow(contract, price, manufacturerName, opCos, industry, contract.CurrentVersionNumber, contract.StartDate));
                }
            }

            // Apply paging
            var totalRows = allRows.Count;
            var totalPages = (int)Math.Ceiling(totalRows / (double)request.PageSize);
            var pagedRows = allRows
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new ContractPricingReportResponse
            {
                Rows = pagedRows,
                TotalRows = totalRows,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }

        private ContractPricingReportRow CreateReportRow(
            Models.Contract contract,
            Models.ContractPrice price,
            string manufacturerName,
            string opCos,
            string? industry,
            int versionNumber,
            DateTime? effectiveDate)
        {
            return new ContractPricingReportRow
            {
                ContractId = contract.Id,
                ContractNumber = contract.ForeignContractId ?? contract.Id.ToString(),
                Manufacturer = manufacturerName,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                OpCos = opCos,
                Industry = industry,
                ContractVersionNumber = versionNumber,
                EffectiveDate = effectiveDate,
                ProductCode = price.Product?.SKU ?? price.Product?.ManufacturerProductCode ?? "",
                ProductName = price.Product?.Name ?? "",
                PricingVersionNumber = price.VersionNumber,
                Allowance = price.Allowance,
                CommercialDelivered = price.CommercialDelPrice,
                CommodityDelivered = price.CommodityDelPrice,
                CommercialFOB = price.CommercialFobPrice,
                CommodityFOB = price.CommodityFobPrice,
                UOM = price.UOM,
                EstimatedVolume = price.EstimatedQty,
                ActualVolume = null // TODO: Calculate from velocity data
            };
        }
    }
}

