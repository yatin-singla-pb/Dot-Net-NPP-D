namespace NPPContractManagement.API.DTOs
{
    public class ContractVersionPriceDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        // Derived single price for backward-compat screens
        public decimal? Price { get; set; }
        public string? PriceType { get; set; }
        public string? UOM { get; set; }
        public string? Tier { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Extended detailed fields aligned with ContractVersionPrice table
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public decimal? EstimatedQty { get; set; }
        public bool BillbacksAllowed { get; set; }
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
    }

    public class ContractVersionDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int VersionNumber { get; set; }
        // New schema fields
        public string Name { get; set; } = string.Empty;
        public string? ForeignContractId { get; set; }
        public bool SendToPerformance { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata
        public string? ManufacturerReferenceNumber { get; set; }
        public string? ManufacturerBillbackName { get; set; }
        public string? ManufacturerTermsAndConditions { get; set; }
        public string? ManufacturerNotes { get; set; }
        public string? ContactPerson { get; set; }
        public string? EntegraContractType { get; set; }
        public string? EntegraVdaProgram { get; set; }
        // Common
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public List<ContractVersionPriceDto> Prices { get; set; } = new();
    }

    public class CreateContractVersionPriceRequest
    {
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public string? PriceType { get; set; }
        public string? UOM { get; set; }
        public string? Tier { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Extended detailed fields to support Save & Update cloning flow
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }
        public decimal? EstimatedQty { get; set; }
        public bool BillbacksAllowed { get; set; }
        public decimal? PUA { get; set; }
        public decimal? FFSPrice { get; set; }
        public decimal? NOIPrice { get; set; }
        public decimal? PTV { get; set; }
        public string? InternalNotes { get; set; }
    }

    public class CreateContractVersionRequest
    {
        // New schema
        public string Name { get; set; } = string.Empty;
        public string? ForeignContractId { get; set; }
        public bool SendToPerformance { get; set; } = false;
        public bool IsSuspended { get; set; } = false;
        public DateTime? SuspendedDate { get; set; }
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata
        public string? ManufacturerReferenceNumber { get; set; }
        public string? ManufacturerBillbackName { get; set; }
        public string? ManufacturerTermsAndConditions { get; set; }
        public string? ManufacturerNotes { get; set; }
        public string? ContactPerson { get; set; }
        public string? EntegraContractType { get; set; }
        public string? EntegraVdaProgram { get; set; }
        // Common
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CreateContractVersionPriceRequest> Prices { get; set; } = new();
        public int? SourceVersionId { get; set; }
    }

    public class UpdateContractVersionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? ForeignContractId { get; set; }
        public bool SendToPerformance { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata
        public string? ManufacturerReferenceNumber { get; set; }
        public string? ManufacturerBillbackName { get; set; }
        public string? ManufacturerTermsAndConditions { get; set; }
        public string? ManufacturerNotes { get; set; }
        public string? ContactPerson { get; set; }
        public string? EntegraContractType { get; set; }
        public string? EntegraVdaProgram { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

