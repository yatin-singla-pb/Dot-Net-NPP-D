using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs.Proposals;
using NPPContractManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace NPPContractManagement.API.Services
{
    public interface IProposalProductExcelService
    {
        Task<byte[]> GenerateTemplateAsync(int manufacturerId);
        Task<byte[]> ExportProposalProductsAsync(int proposalId);
        Task<ProposalProductExcelImportResponse> ImportFromExcelAsync(Stream excelStream, int manufacturerId);
    }

    public class ProposalProductExcelService : IProposalProductExcelService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProposalProductExcelService> _logger;

        public ProposalProductExcelService(
            ApplicationDbContext context,
            ILogger<ProposalProductExcelService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<byte[]> GenerateTemplateAsync(int manufacturerId)
        {
            // Get all products for the manufacturer
            var products = await _context.Products
                .Where(p => p.ManufacturerId == manufacturerId)
                .OrderBy(p => p.SKU)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Products");

            // Define headers
            var headers = new[]
            {
                "SKU", "Product Name", "UOM", "Billbacks Allowed",
                "Allowance", "Commercial Del Price", "Commercial FOB Price",
                "Commodity Del Price", "Commodity FOB Price", "PUA",
                "FFS Price", "NOI Price", "PTV",
                "Internal Notes", "Manufacturer Notes"
            };

            // Write headers
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Write product data
            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cells[row, 1].Value = product.ManufacturerProductCode ?? product.SKU;
                worksheet.Cells[row, 2].Value = product.Description ?? product.Name;
                // Leave pricing columns empty for user to fill
                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Set minimum column widths
            for (int i = 1; i <= headers.Length; i++)
            {
                if (worksheet.Column(i).Width < 15)
                    worksheet.Column(i).Width = 15;
            }

            return package.GetAsByteArray();
        }

        public async Task<byte[]> ExportProposalProductsAsync(int proposalId)
        {
            var proposal = await _context.Set<NPPContractManagement.API.Domain.Proposals.Entities.Proposal>()
                .Include(p => p.Products)
                    .ThenInclude(pp => pp.Product)
                .FirstOrDefaultAsync(p => p.Id == proposalId);

            if (proposal == null)
                throw new ArgumentException($"Proposal {proposalId} not found");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Proposal Products");

            var headers = new[]
            {
                "SKU", "Product Name", "UOM", "Billbacks Allowed",
                "Allowance", "Commercial Del Price", "Commercial FOB Price",
                "Commodity Del Price", "Commodity FOB Price", "PUA",
                "FFS Price", "NOI Price", "PTV",
                "Internal Notes", "Manufacturer Notes"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int row = 2;
            foreach (var pp in proposal.Products.OrderBy(p => p.Product?.ManufacturerProductCode ?? p.Product?.SKU))
            {
                worksheet.Cells[row, 1].Value = pp.Product?.ManufacturerProductCode ?? pp.Product?.SKU;
                worksheet.Cells[row, 2].Value = pp.Product?.Description ?? pp.Product?.Name;
                worksheet.Cells[row, 3].Value = pp.Uom;
                worksheet.Cells[row, 4].Value = pp.BillbacksAllowed ? "Yes" : "No";
                worksheet.Cells[row, 5].Value = pp.Allowance;
                worksheet.Cells[row, 6].Value = pp.CommercialDelPrice;
                worksheet.Cells[row, 7].Value = pp.CommercialFobPrice;
                worksheet.Cells[row, 8].Value = pp.CommodityDelPrice;
                worksheet.Cells[row, 9].Value = pp.CommodityFobPrice;
                worksheet.Cells[row, 10].Value = pp.Pua;
                worksheet.Cells[row, 11].Value = pp.FfsPrice;
                worksheet.Cells[row, 12].Value = pp.NoiPrice;
                worksheet.Cells[row, 13].Value = pp.Ptv;
                worksheet.Cells[row, 14].Value = pp.InternalNotes;
                worksheet.Cells[row, 15].Value = pp.ManufacturerNotes;
                row++;
            }

            worksheet.Cells[worksheet.Dimension?.Address ?? "A1"].AutoFitColumns();
            for (int i = 1; i <= headers.Length; i++)
            {
                if (worksheet.Column(i).Width < 15)
                    worksheet.Column(i).Width = 15;
            }

            return package.GetAsByteArray();
        }

        public async Task<ProposalProductExcelImportResponse> ImportFromExcelAsync(Stream excelStream, int manufacturerId)
        {
            var response = new ProposalProductExcelImportResponse
            {
                Success = true,
                Message = "Import completed successfully"
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(excelStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                response.Success = false;
                response.Message = "No worksheet found in Excel file";
                return response;
            }

            // Get all products for the manufacturer and build lookup by SKU and ManufacturerProductCode
            var allProducts = await _context.Products
                .Where(p => p.ManufacturerId == manufacturerId)
                .ToListAsync();

            // Build a combined lookup: try SKU first, then ManufacturerProductCode
            var manufacturerProducts = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in allProducts)
            {
                // Index by ManufacturerProductCode first (lower priority, gets overwritten by SKU if same)
                if (!string.IsNullOrWhiteSpace(p.ManufacturerProductCode) && !manufacturerProducts.ContainsKey(p.ManufacturerProductCode))
                    manufacturerProducts[p.ManufacturerProductCode] = p;

                // Index by SKU (higher priority)
                if (!string.IsNullOrWhiteSpace(p.SKU) && !manufacturerProducts.ContainsKey(p.SKU))
                    manufacturerProducts[p.SKU] = p;
            }

            int rowCount = worksheet.Dimension?.Rows ?? 0;
            response.TotalRows = rowCount - 1; // Exclude header

            // Start from row 2 (skip header)
            for (int row = 2; row <= rowCount; row++)
            {
                var importRow = ParseRow(worksheet, row, manufacturerProducts, manufacturerId);
                
                if (importRow.IsValid)
                {
                    response.ImportedProducts.Add(importRow);
                    response.ValidRows++;
                }
                else
                {
                    response.InvalidRows++;
                    response.ValidationErrors.Add($"Row {row}: {importRow.ValidationError}");
                }
            }

            if (response.ValidRows == 0 && response.InvalidRows > 0)
            {
                response.Success = false;
                response.Message = $"No products could be imported. {response.InvalidRows} row(s) had errors.";
            }
            else if (response.InvalidRows > 0)
            {
                // Partial success — valid rows are returned, invalid ones listed as errors
                response.Success = true;
                response.Message = $"Imported {response.ValidRows} product(s). {response.InvalidRows} row(s) were skipped due to errors.";
            }

            return response;
        }

        private ProposalProductImportRow ParseRow(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, Product> manufacturerProducts,
            int manufacturerId)
        {
            var importRow = new ProposalProductImportRow
            {
                RowNumber = row,
                IsValid = true
            };

            try
            {
                // Read SKU
                var sku = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(sku))
                {
                    importRow.IsValid = false;
                    importRow.ValidationError = "SKU is required";
                    return importRow;
                }

                importRow.SKU = sku;

                // Validate product exists and belongs to manufacturer (matches by SKU or ManufacturerProductCode)
                if (!manufacturerProducts.TryGetValue(sku, out var product))
                {
                    importRow.IsValid = false;
                    importRow.ValidationError = $"Product with code '{sku}' not found for the selected manufacturer";
                    return importRow;
                }

                importRow.ProductId = product.Id;
                importRow.ProductName = product.Description ?? product.Name;

                // Read pricing fields
                importRow.UOM = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                importRow.BillbacksAllowed = ParseBoolean(worksheet.Cells[row, 4].Value);
                importRow.Allowance = ParseDecimal(worksheet.Cells[row, 5].Value);
                importRow.CommercialDelPrice = ParseDecimal(worksheet.Cells[row, 6].Value);
                importRow.CommercialFobPrice = ParseDecimal(worksheet.Cells[row, 7].Value);
                importRow.CommodityDelPrice = ParseDecimal(worksheet.Cells[row, 8].Value);
                importRow.CommodityFobPrice = ParseDecimal(worksheet.Cells[row, 9].Value);
                importRow.PUA = ParseDecimal(worksheet.Cells[row, 10].Value);
                importRow.FFSPrice = ParseDecimal(worksheet.Cells[row, 11].Value);
                importRow.NOIPrice = ParseDecimal(worksheet.Cells[row, 12].Value);
                importRow.PTV = ParseDecimal(worksheet.Cells[row, 13].Value);
                importRow.InternalNotes = worksheet.Cells[row, 14].Value?.ToString()?.Trim();
                importRow.ManufacturerNotes = worksheet.Cells[row, 15].Value?.ToString()?.Trim();
            }
            catch (Exception ex)
            {
                importRow.IsValid = false;
                importRow.ValidationError = $"Error parsing row: {ex.Message}";
            }

            return importRow;
        }

        private bool ParseBoolean(object? value)
        {
            if (value == null) return false;

            var str = value.ToString()?.Trim().ToLowerInvariant();
            return str == "true" || str == "yes" || str == "1" || str == "y";
        }

        private decimal? ParseDecimal(object? value)
        {
            if (value == null) return null;

            if (decimal.TryParse(value.ToString(), out var result))
                return result;

            return null;
        }
    }
}

