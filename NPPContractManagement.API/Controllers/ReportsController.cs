using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs.Reports;
using NPPContractManagement.API.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "System Administrator,Contract Manager,Manufacturer")]
    public class ReportsController : ControllerBase
    {
        private readonly IContractOverTermReportService _reportService;
        private readonly IContractPricingReportService _pricingReportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IContractOverTermReportService reportService,
            IContractPricingReportService pricingReportService,
            ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _pricingReportService = pricingReportService;
            _logger = logger;
        }

        /// <summary>
        /// Generate Contract Over Term Report
        /// </summary>
        [HttpPost("contract-over-term")]
        public async Task<ActionResult<ContractOverTermReportResponse>> GetContractOverTermReport(
            [FromBody] ContractOverTermReportRequest request)
        {
            try
            {
                var response = await _reportService.GenerateReportAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Contract Over Term report");
                return StatusCode(500, new { message = "Error generating report", error = ex.Message });
            }
        }

        /// <summary>
        /// Download Contract Over Term Report as Excel
        /// </summary>
        [HttpPost("contract-over-term/excel")]
        public async Task<IActionResult> DownloadContractOverTermReportExcel(
            [FromBody] ContractOverTermReportRequest request)
        {
            try
            {
                var reportData = await _reportService.GenerateReportAsync(request);
                var excelBytes = GenerateExcelFile(reportData);
                
                var fileName = $"Contract_Over_Term_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Contract Over Term Excel report");
                return StatusCode(500, new { message = "Error generating Excel report", error = ex.Message });
            }
        }

        private byte[] GenerateExcelFile(ContractOverTermReportResponse reportData)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Contract Over Term Report");

            // Define base headers
            var baseHeaders = new List<string>
            {
                "Contract #",
                "Manufacturer",
                "Start Date",
                "End Date",
                "Op-Cos",
                "Product Code",
                "Product Name",
                "Pricing",
                "Estimated Volume",
                "Actual Volume",
                "Industry",
                "Price Type"
            };

            // Add previous term headers dynamically
            var headers = new List<string>(baseHeaders);
            for (int i = 1; i <= reportData.MaxTermsBack; i++)
            {
                headers.Add($"Previous Term {i} Start Date");
                headers.Add($"Previous Term {i} End Date");
                headers.Add($"Previous Term {i} Pricing");
                headers.Add($"Previous Term {i} Estimated Volume");
                headers.Add($"Previous Term {i} Actual Volume");
            }

            // Write headers
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Write data rows
            int row = 2;
            foreach (var dataRow in reportData.Rows)
            {
                int col = 1;
                
                // Base columns
                worksheet.Cells[row, col++].Value = dataRow.ContractNumber;
                worksheet.Cells[row, col++].Value = dataRow.Manufacturer;
                worksheet.Cells[row, col++].Value = dataRow.StartDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col++].Value = dataRow.EndDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col++].Value = dataRow.OpCos;
                worksheet.Cells[row, col++].Value = dataRow.ProductCode;
                worksheet.Cells[row, col++].Value = dataRow.ProductName;
                worksheet.Cells[row, col++].Value = dataRow.Pricing;
                worksheet.Cells[row, col++].Value = dataRow.EstimatedVolume;
                worksheet.Cells[row, col++].Value = dataRow.ActualVolume;
                worksheet.Cells[row, col++].Value = dataRow.Industry;
                worksheet.Cells[row, col++].Value = dataRow.PriceType;

                // Previous terms columns
                for (int i = 1; i <= reportData.MaxTermsBack; i++)
                {
                    var prevTerm = dataRow.PreviousTerms.FirstOrDefault(pt => pt.TermNumber == i);
                    if (prevTerm != null)
                    {
                        worksheet.Cells[row, col++].Value = prevTerm.StartDate?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, col++].Value = prevTerm.EndDate?.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, col++].Value = prevTerm.Pricing;
                        worksheet.Cells[row, col++].Value = prevTerm.EstimatedVolume;
                        worksheet.Cells[row, col++].Value = prevTerm.ActualVolume;
                    }
                    else
                    {
                        col += 5; // Skip 5 columns for this term
                    }
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }

        /// <summary>
        /// Generate Contract Pricing Report
        /// </summary>
        [HttpPost("contract-pricing")]
        public async Task<ActionResult<ContractPricingReportResponse>> GetContractPricingReport(
            [FromBody] ContractPricingReportRequest request)
        {
            try
            {
                // Get manufacturer IDs from claims for manufacturer users
                IEnumerable<int>? allowedManufacturerIds = null;
                var roleClaims = User?.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList() ?? new List<string>();
                if (roleClaims.Contains("Manufacturer"))
                {
                    var claim = User?.FindFirst("manufacturer_ids")?.Value;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(claim))
                        {
                            allowedManufacturerIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(claim);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse manufacturer_ids claim");
                    }
                }

                var response = await _pricingReportService.GenerateReportAsync(request, allowedManufacturerIds);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Contract Pricing report");
                return StatusCode(500, new { message = "Error generating report", error = ex.Message });
            }
        }

        /// <summary>
        /// Download Contract Pricing Report as Excel
        /// </summary>
        [HttpPost("contract-pricing/excel")]
        public async Task<IActionResult> DownloadContractPricingReportExcel(
            [FromBody] ContractPricingReportRequest request)
        {
            try
            {
                // Get manufacturer IDs from claims for manufacturer users
                IEnumerable<int>? allowedManufacturerIds = null;
                var roleClaims = User?.FindAll(System.Security.Claims.ClaimTypes.Role)?.Select(r => r.Value).ToList() ?? new List<string>();
                if (roleClaims.Contains("Manufacturer"))
                {
                    var claim = User?.FindFirst("manufacturer_ids")?.Value;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(claim))
                        {
                            allowedManufacturerIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(claim);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse manufacturer_ids claim");
                    }
                }

                var reportData = await _pricingReportService.GenerateReportAsync(request, allowedManufacturerIds);
                var excelBytes = GenerateContractPricingExcelFile(reportData);

                var fileName = $"Contract_Pricing_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Contract Pricing Excel report");
                return StatusCode(500, new { message = "Error generating Excel report", error = ex.Message });
            }
        }

        private byte[] GenerateContractPricingExcelFile(ContractPricingReportResponse reportData)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Contract Pricing Report");

            // Header row
            var headers = new[]
            {
                "Contract #", "Manufacturer", "Start Date", "End Date", "Op-Cos", "Industry",
                "Contract Version", "Effective Date", "Product Code", "Product Name",
                "Pricing Version", "Allowance", "Commercial Delivered", "Commodity Delivered",
                "Commercial FOB", "Commodity FOB", "UOM", "Estimated Volume", "Actual Volume"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Data rows
            int row = 2;
            foreach (var dataRow in reportData.Rows)
            {
                int col = 1;
                worksheet.Cells[row, col++].Value = dataRow.ContractNumber;
                worksheet.Cells[row, col++].Value = dataRow.Manufacturer;
                worksheet.Cells[row, col++].Value = dataRow.StartDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col++].Value = dataRow.EndDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col++].Value = dataRow.OpCos;
                worksheet.Cells[row, col++].Value = dataRow.Industry;
                worksheet.Cells[row, col++].Value = dataRow.ContractVersionNumber;
                worksheet.Cells[row, col++].Value = dataRow.EffectiveDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col++].Value = dataRow.ProductCode;
                worksheet.Cells[row, col++].Value = dataRow.ProductName;
                worksheet.Cells[row, col++].Value = dataRow.PricingVersionNumber;
                worksheet.Cells[row, col++].Value = dataRow.Allowance;
                worksheet.Cells[row, col++].Value = dataRow.CommercialDelivered;
                worksheet.Cells[row, col++].Value = dataRow.CommodityDelivered;
                worksheet.Cells[row, col++].Value = dataRow.CommercialFOB;
                worksheet.Cells[row, col++].Value = dataRow.CommodityFOB;
                worksheet.Cells[row, col++].Value = dataRow.UOM;
                worksheet.Cells[row, col++].Value = dataRow.EstimatedVolume;
                worksheet.Cells[row, col++].Value = dataRow.ActualVolume;

                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}

