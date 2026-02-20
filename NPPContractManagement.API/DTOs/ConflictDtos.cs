namespace NPPContractManagement.API.DTOs
{
    public class ProposalConflictResultDto
    {
        public int ProposalId { get; set; }
        public bool HasConflicts { get; set; }
        public int TotalConflictCount { get; set; }
        public List<ProductConflictDto> Conflicts { get; set; } = new();
    }

    public class ProductConflictDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ManufacturerProductCode { get; set; }

        public int ConflictingContractId { get; set; }
        public string ConflictingContractName { get; set; } = string.Empty;
        public int ConflictingContractVersionNumber { get; set; }
        public string? ConflictingContractForeignId { get; set; }
        public string? ConflictingManufacturerName { get; set; }

        public List<OpCoConflictDto> OverlappingOpCos { get; set; } = new();
        public bool IsNationwideConflict { get; set; }

        public DateTime ProposalStartDate { get; set; }
        public DateTime ProposalEndDate { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public DateTime OverlapStartDate { get; set; }
        public DateTime OverlapEndDate { get; set; }
    }

    public class OpCoConflictDto
    {
        public int OpCoId { get; set; }
        public string OpCoName { get; set; } = string.Empty;
    }
}
