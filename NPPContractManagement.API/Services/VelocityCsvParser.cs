using NPPContractManagement.API.DTOs;
using System.Globalization;
using System.Text;

namespace NPPContractManagement.API.Services
{
    public interface IVelocityCsvParser
    {
        Task<List<VelocityValidationResult>> ParseAndValidateAsync(Stream csvStream);
        Task<List<VelocityShipmentCsvRow>> ParseCsvAsync(Stream csvStream);
        VelocityValidationResult ValidateRow(VelocityShipmentCsvRow row, int rowIndex);
    }

    public class VelocityCsvParser : IVelocityCsvParser
    {
        private readonly ILogger<VelocityCsvParser> _logger;

        // New 22-field format - no required columns (all optional except what distributor provides)
        private static readonly string[] ExpectedColumns = new[]
        {
            "OPCO", "Customer #", "Customer Name", "Address One", "Address Two",
            "City", "Zip Code", "Invoice #", "Invoice Date", "Product #",
            "Brand", "Pack Size", "Description", "Corp Manuf #", "GTIN",
            "Manufacturer Name", "Qty", "Sales", "Landed Cost", "Allowances",
            "Freight1", "Freight2"
        };

        public VelocityCsvParser(ILogger<VelocityCsvParser> logger)
        {
            _logger = logger;
        }

        public async Task<List<VelocityValidationResult>> ParseAndValidateAsync(Stream csvStream)
        {
            var results = new List<VelocityValidationResult>();
            var rows = await ParseCsvAsync(csvStream);

            for (int i = 0; i < rows.Count; i++)
            {
                var validationResult = ValidateRow(rows[i], i + 2); // +2 because row 1 is header, index starts at 0
                results.Add(validationResult);
            }

            return results;
        }

        public async Task<List<VelocityShipmentCsvRow>> ParseCsvAsync(Stream csvStream)
        {
            var rows = new List<VelocityShipmentCsvRow>();

            using var reader = new StreamReader(csvStream, Encoding.UTF8, leaveOpen: true);

            // Read header
            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new InvalidOperationException("CSV file is empty");
            }

            var headers = ParseCsvLine(headerLine);
            // Note: No strict header validation - all fields are optional

            // Read data rows
            string? line;
            int rowNum = 0;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = ParseCsvLine(line);

                // Parse 22 fields by position (0-based index)
                var row = new VelocityShipmentCsvRow
                {
                    OpCo = GetField(values, 0),
                    CustomerNumber = GetField(values, 1),
                    CustomerName = GetField(values, 2),
                    AddressOne = GetField(values, 3),
                    AddressTwo = GetField(values, 4),
                    City = GetField(values, 5),
                    ZipCode = GetField(values, 6),
                    InvoiceNumber = GetField(values, 7),
                    InvoiceDate = GetField(values, 8),
                    ProductNumber = GetField(values, 9),
                    Brand = GetField(values, 10),
                    PackSize = GetField(values, 11),
                    Description = GetField(values, 12),
                    CorpManufNumber = GetField(values, 13),
                    GTIN = GetField(values, 14),
                    ManufacturerName = GetField(values, 15),
                    Qty = GetField(values, 16),
                    Sales = GetField(values, 17),
                    LandedCost = GetField(values, 18),
                    Allowances = GetField(values, 19),
                    Freight1 = GetField(values, 20),
                    Freight2 = GetField(values, 21)
                };

                rows.Add(row);
                rowNum++;
            }

            _logger.LogInformation("Parsed {RowCount} rows from CSV", rowNum);
            return rows;
        }

        public VelocityValidationResult ValidateRow(VelocityShipmentCsvRow row, int rowIndex)
        {
            var result = new VelocityValidationResult
            {
                Row = row,
                IsValid = true
            };

            // Minimal validation - no required fields per requirements
            // Just validate data types if values are provided

            // Validate Qty if provided
            if (!string.IsNullOrWhiteSpace(row.Qty))
            {
                if (!int.TryParse(row.Qty, out int qty) || qty < 0)
                {
                    result.Errors.Add($"Row {rowIndex}: Qty must be a valid non-negative integer");
                    result.IsValid = false;
                }
            }

            // Validate Sales if provided
            if (!string.IsNullOrWhiteSpace(row.Sales))
            {
                if (!decimal.TryParse(row.Sales, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    result.Errors.Add($"Row {rowIndex}: Sales must be a valid decimal number");
                    result.IsValid = false;
                }
            }

            // Validate LandedCost if provided
            if (!string.IsNullOrWhiteSpace(row.LandedCost))
            {
                if (!decimal.TryParse(row.LandedCost, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    result.Errors.Add($"Row {rowIndex}: Landed Cost must be a valid decimal number");
                    result.IsValid = false;
                }
            }

            // Validate Allowances if provided
            if (!string.IsNullOrWhiteSpace(row.Allowances))
            {
                if (!decimal.TryParse(row.Allowances, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    result.Errors.Add($"Row {rowIndex}: Allowances must be a valid decimal number");
                    result.IsValid = false;
                }
            }

            // Validate InvoiceDate if provided
            if (!string.IsNullOrWhiteSpace(row.InvoiceDate))
            {
                if (!DateTime.TryParse(row.InvoiceDate, out _))
                {
                    result.Errors.Add($"Row {rowIndex}: Invoice Date must be a valid date");
                    result.IsValid = false;
                }
            }

            return result;
        }

        private string[] ParseCsvLine(string line)
        {
            // Simple CSV parser - handles quoted fields
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString());
            return values.ToArray();
        }

        private string? GetField(string[] values, int index)
        {
            if (index >= 0 && index < values.Length)
            {
                var value = values[index].Trim();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            return null;
        }
    }
}

