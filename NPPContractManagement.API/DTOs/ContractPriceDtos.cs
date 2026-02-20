namespace NPPContractManagement.API.DTOs
{
    public class ContractPriceDto
    {
        public int Id { get; set; }
        public int VersionNumber { get; set; }
        public int ContractId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public string UOM { get; set; } = string.Empty;
        public decimal? EstimatedQty { get; set; }
        public bool BillbacksAllowed { get; set; }
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateContractPriceRequest
    {
        public int VersionNumber { get; set; }
        public int ContractId { get; set; }
        public int ProductId { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public string UOM { get; set; } = string.Empty;
        public decimal? EstimatedQty { get; set; }
        public bool BillbacksAllowed { get; set; } = false;
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
    }

    public class UpdateContractPriceRequest
    {
        public int ContractId { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public string UOM { get; set; } = string.Empty;
        public decimal? EstimatedQty { get; set; }
        public bool BillbacksAllowed { get; set; }
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
    }
}

