namespace NPPContractManagement.API.DTOs
{
    public class VelocityShipmentDto
    {
        public int Id { get; set; }
        public int DistributorId { get; set; }
        public string? DistributorName { get; set; }
        public string ShipmentId { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime ShippedAt { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public int? VelocityJobId { get; set; }
        public int? RowIndex { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Represents a single row from the new 22-field CSV format
    /// </summary>
    public class VelocityShipmentCsvRow
    {
        public string? OpCo { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? AddressOne { get; set; }
        public string? AddressTwo { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? InvoiceDate { get; set; }
        public string? ProductNumber { get; set; }
        public string? Brand { get; set; }
        public string? PackSize { get; set; }
        public string? Description { get; set; }
        public string? CorpManufNumber { get; set; }
        public string? GTIN { get; set; }
        public string? ManufacturerName { get; set; }
        public string? Qty { get; set; }
        public string? Sales { get; set; }
        public string? LandedCost { get; set; }
        public string? Allowances { get; set; }
        public string? Freight1 { get; set; }
        public string? Freight2 { get; set; }
    }

    public class VelocityValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public VelocityShipmentCsvRow? Row { get; set; }
    }
}

