namespace NPPContractManagement.API.DTOs.Proposals
{
    /// <summary>
    /// Response from Excel template generation
    /// </summary>
    public class ProposalProductExcelTemplateResponse
    {
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// Response from Excel import
    /// </summary>
    public class ProposalProductExcelImportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
        public List<ProposalProductImportRow> ImportedProducts { get; set; } = new();
        public List<string> ValidationErrors { get; set; } = new();
    }

    /// <summary>
    /// Represents a single imported product row
    /// </summary>
    public class ProposalProductImportRow
    {
        public int RowNumber { get; set; }
        public int ProductId { get; set; }
        public string? SKU { get; set; }
        public string? ProductName { get; set; }
        public string? UOM { get; set; }
        public bool BillbacksAllowed { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
        public string? ManufacturerNotes { get; set; }
        public bool IsValid { get; set; }
        public string? ValidationError { get; set; }
    }
}

