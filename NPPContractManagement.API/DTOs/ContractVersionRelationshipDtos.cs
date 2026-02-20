namespace NPPContractManagement.API.DTOs
{
    public class BaseVersionAssignmentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int VersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
    public class UpdateAssignedRequest { public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }


    public class ContractIndustryVersionDto : BaseVersionAssignmentDto { public int IndustryId { get; set; } }
    public class ContractManufacturerVersionDto : BaseVersionAssignmentDto { public int ManufacturerId { get; set; } }
    public class ContractOpCoVersionDto : BaseVersionAssignmentDto { public int OpCoId { get; set; } }
    public class ContractDistributorVersionDto : BaseVersionAssignmentDto { public int DistributorId { get; set; } }
    public class ContractVersionProductDto : BaseVersionAssignmentDto { public int ProductId { get; set; } }

    public class ContractVersionPriceEntryDto : BaseVersionAssignmentDto
    {
        public int PriceId { get; set; }
        public int ProductId { get; set; }
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

    public class CreateContractIndustryVersionRequest { public int ContractId { get; set; } public int IndustryId { get; set; } public int VersionNumber { get; set; } public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }
    public class CreateContractManufacturerVersionRequest { public int ContractId { get; set; } public int ManufacturerId { get; set; } public int VersionNumber { get; set; } public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }
    public class CreateContractOpCoVersionRequest { public int ContractId { get; set; } public int OpCoId { get; set; } public int VersionNumber { get; set; } public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }
    public class CreateContractDistributorVersionRequest { public int ContractId { get; set; } public int DistributorId { get; set; } public int VersionNumber { get; set; } public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }
    public class CreateContractVersionProductRequest { public int ContractId { get; set; } public int ProductId { get; set; } public int VersionNumber { get; set; } public string? AssignedBy { get; set; } public DateTime? AssignedDate { get; set; } }

    public class CreateContractVersionPriceEntryRequest
    {
        public int ContractId { get; set; }
        public int PriceId { get; set; }
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
        public int VersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class UpdateContractVersionPriceEntryRequest : CreateContractVersionPriceEntryRequest {}
}

