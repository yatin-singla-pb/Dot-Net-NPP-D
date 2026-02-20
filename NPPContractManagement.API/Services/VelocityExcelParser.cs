using NPPContractManagement.API.DTOs;
using OfficeOpenXml;
using System.Globalization;

namespace NPPContractManagement.API.Services
{
    public interface IVelocityExcelParser
    {
        Task<List<VelocityValidationResult>> ParseAndValidateAsync(Stream excelStream);
        Task<List<VelocityShipmentCsvRow>> ParseExcelAsync(Stream excelStream);
    }

    public class VelocityExcelParser : IVelocityExcelParser
    {
        private readonly ILogger<VelocityExcelParser> _logger;
        private readonly IVelocityCsvParser _csvParser;

        public VelocityExcelParser(ILogger<VelocityExcelParser> logger, IVelocityCsvParser csvParser)
        {
            _logger = logger;
            _csvParser = csvParser;
        }

        public async Task<List<VelocityValidationResult>> ParseAndValidateAsync(Stream excelStream)
        {
            var results = new List<VelocityValidationResult>();
            var rows = await ParseExcelAsync(excelStream);

            for (int i = 0; i < rows.Count; i++)
            {
                var validationResult = _csvParser.ValidateRow(rows[i], i + 2); // +2 because row 1 is header
                results.Add(validationResult);
            }

            return results;
        }

        public async Task<List<VelocityShipmentCsvRow>> ParseExcelAsync(Stream excelStream)
        {
            var rows = new List<VelocityShipmentCsvRow>();

            using var package = new ExcelPackage(excelStream);

            if (package.Workbook.Worksheets.Count == 0)
            {
                throw new InvalidOperationException("Excel file contains no worksheets");
            }

            var worksheet = package.Workbook.Worksheets[0]; // Use first worksheet

            if (worksheet.Dimension == null)
            {
                throw new InvalidOperationException("Excel worksheet is empty");
            }

            var rowCount = worksheet.Dimension.End.Row;

            if (rowCount < 2)
            {
                throw new InvalidOperationException("Excel file must have at least a header row and one data row");
            }

            // Read data rows (starting from row 2) - parse by position (20 fields)
            for (int row = 2; row <= rowCount; row++)
            {
                // Check if row is empty
                bool isEmptyRow = true;
                for (int col = 1; col <= 20; col++)
                {
                    if (worksheet.Cells[row, col].Value != null)
                    {
                        isEmptyRow = false;
                        break;
                    }
                }

                if (isEmptyRow) continue;

                // Parse 22 fields by position (1-based column index in Excel)
                var dataRow = new VelocityShipmentCsvRow
                {
                    OpCo = GetCellValue(worksheet, row, 1),
                    CustomerNumber = GetCellValue(worksheet, row, 2),
                    CustomerName = GetCellValue(worksheet, row, 3),
                    AddressOne = GetCellValue(worksheet, row, 4),
                    AddressTwo = GetCellValue(worksheet, row, 5),
                    City = GetCellValue(worksheet, row, 6),
                    ZipCode = GetCellValue(worksheet, row, 7),
                    InvoiceNumber = GetCellValue(worksheet, row, 8),
                    InvoiceDate = GetCellValue(worksheet, row, 9),
                    ProductNumber = GetCellValue(worksheet, row, 10),
                    Brand = GetCellValue(worksheet, row, 11),
                    PackSize = GetCellValue(worksheet, row, 12),
                    Description = GetCellValue(worksheet, row, 13),
                    CorpManufNumber = GetCellValue(worksheet, row, 14),
                    GTIN = GetCellValue(worksheet, row, 15),
                    ManufacturerName = GetCellValue(worksheet, row, 16),
                    Qty = GetCellValue(worksheet, row, 17),
                    Sales = GetCellValue(worksheet, row, 18),
                    LandedCost = GetCellValue(worksheet, row, 19),
                    Allowances = GetCellValue(worksheet, row, 20),
                    Freight1 = GetCellValue(worksheet, row, 21),
                    Freight2 = GetCellValue(worksheet, row, 22)
                };

                rows.Add(dataRow);
            }

            _logger.LogInformation("Parsed {RowCount} rows from Excel", rows.Count);
            return await Task.FromResult(rows);
        }

        private string? GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            var cellValue = worksheet.Cells[row, col].Value;

            if (cellValue == null)
            {
                return null;
            }

            // Handle DateTime values specially
            if (cellValue is DateTime dateTime)
            {
                // Convert to standard date format
                return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            // Handle numeric values
            if (cellValue is double || cellValue is int || cellValue is decimal)
            {
                return cellValue.ToString() ?? null;
            }

            var stringValue = cellValue.ToString()?.Trim();
            return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue;
        }
    }
}

