using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs.Reports;
using NPPContractManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace NPPContractManagement.API.Services
{
    public interface IContractOverTermReportService
    {
        Task<ContractOverTermReportResponse> GenerateReportAsync(ContractOverTermReportRequest request);
    }

    public class ContractOverTermReportService : IContractOverTermReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContractOverTermReportService> _logger;

        public ContractOverTermReportService(
            ApplicationDbContext context,
            ILogger<ContractOverTermReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ContractOverTermReportResponse> GenerateReportAsync(ContractOverTermReportRequest request)
        {
            var allRows = new List<ContractOverTermReportRow>();

            // Get all contracts that overlap with or are before the point in time
            var contractsQuery = _context.Contracts
                .Include(c => c.ContractManufacturers)
                    .ThenInclude(cm => cm.Manufacturer)
                .Include(c => c.ContractOpCos).ThenInclude(co => co.OpCo)
                .Include(c => c.ContractIndustries).ThenInclude(ci => ci.Industry)
                .Where(c => c.StartDate <= request.PointInTime)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.ContractNumber))
            {
                contractsQuery = contractsQuery.Where(c => c.ForeignContractId != null &&
                    c.ForeignContractId.Contains(request.ContractNumber));
            }

            if (request.ManufacturerId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.ContractManufacturers.Any(cm => cm.ManufacturerId == request.ManufacturerId.Value));
            }

            if (request.OpCoId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.ContractOpCos.Any(co => co.OpCoId == request.OpCoId.Value));
            }

            var contracts = await contractsQuery.ToListAsync();

            // For each contract, get the pricing data
            foreach (var contract in contracts)
            {
                var contractPrices = await _context.ContractVersionPrices
                    .Include(cvp => cvp.Product)
                    .Where(cvp => cvp.ContractId == contract.Id && cvp.VersionNumber == contract.CurrentVersionNumber)
                    .ToListAsync();

                // Apply product filter if specified
                if (request.ProductId.HasValue)
                {
                    contractPrices = contractPrices.Where(cvp => cvp.ProductId == request.ProductId.Value).ToList();
                }

                // Get OpCos as comma-separated string
                var opCos = string.Join(", ", contract.ContractOpCos.Select(co => co.OpCo?.Name ?? ""));

                // Get industry (first one if multiple)
                var industry = contract.ContractIndustries.FirstOrDefault()?.Industry?.Name;

                foreach (var price in contractPrices)
                {
                    // Get manufacturer name from ContractManufacturers
                    var manufacturerName = contract.ContractManufacturers?.FirstOrDefault()?.Manufacturer?.Name ?? "";

                    var row = new ContractOverTermReportRow
                    {
                        ContractNumber = contract.ForeignContractId ?? contract.Id.ToString(),
                        Manufacturer = manufacturerName,
                        StartDate = contract.StartDate,
                        EndDate = contract.EndDate,
                        OpCos = opCos,
                        ProductCode = price.Product?.SKU ?? "",
                        ProductName = price.Product?.Name ?? "",
                        Pricing = GetPrimaryPrice(price),
                        EstimatedVolume = price.EstimatedQty,
                        ActualVolume = null, // TODO: Get from velocity data
                        Industry = industry,
                        PriceType = price.PriceType
                    };

                    // Get previous terms for this contract/product combination
                    row.PreviousTerms = await GetPreviousTermsAsync(
                        contract.Id,
                        price.ProductId,
                        contract.StartDate,
                        request.MaxTermsBack);

                    allRows.Add(row);
                }
            }

            // Apply paging
            var totalRows = allRows.Count;
            var totalPages = (int)Math.Ceiling(totalRows / (double)request.PageSize);
            var pagedRows = allRows
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var response = new ContractOverTermReportResponse
            {
                Rows = pagedRows,
                TotalRows = totalRows,
                MaxTermsBack = request.MaxTermsBack,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return response;
        }

        private async Task<List<PreviousTermData>> GetPreviousTermsAsync(
            int contractId, 
            int productId, 
            DateTime? currentStartDate, 
            int maxTerms)
        {
            var previousTerms = new List<PreviousTermData>();

            if (!currentStartDate.HasValue)
                return previousTerms;

            // Get all versions of this contract that ended before the current start date
            var previousVersions = await _context.ContractVersions
                .Where(cv => cv.ContractId == contractId && 
                             cv.EndDate < currentStartDate.Value)
                .OrderByDescending(cv => cv.EndDate)
                .Take(maxTerms)
                .ToListAsync();

            int termNumber = 1;
            foreach (var version in previousVersions)
            {
                // Get pricing for this version and product
                var price = await _context.ContractVersionPrices
                    .FirstOrDefaultAsync(cvp => cvp.ContractId == contractId && 
                                                cvp.VersionNumber == version.VersionNumber && 
                                                cvp.ProductId == productId);

                if (price != null)
                {
                    previousTerms.Add(new PreviousTermData
                    {
                        TermNumber = termNumber,
                        StartDate = version.StartDate,
                        EndDate = version.EndDate,
                        Pricing = GetPrimaryPrice(price),
                        EstimatedVolume = price.EstimatedQty,
                        ActualVolume = null // TODO: Get from velocity data
                    });

                    termNumber++;
                }
            }

            return previousTerms;
        }

        /// <summary>
        /// Get the primary price from a contract version price
        /// Priority: Commercial Del > Commercial FOB > Commodity Del > Commodity FOB > PUA
        /// </summary>
        private decimal? GetPrimaryPrice(ContractVersionPrice price)
        {
            return price.CommercialDelPrice
                ?? price.CommercialFobPrice
                ?? price.CommodityDelPrice
                ?? price.CommodityFobPrice
                ?? price.PUA;
        }
    }
}

