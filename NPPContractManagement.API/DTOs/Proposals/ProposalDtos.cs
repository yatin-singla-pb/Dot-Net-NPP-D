using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs.Proposals
{
    public class ProposalDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ProposalTypeId { get; set; }
        public int ProposalStatusId { get; set; }
        public string? ProposalStatusName { get; set; }
        public int? ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? InternalNotes { get; set; }
        public string? RejectReason { get; set; }
        public bool IsActive { get; set; }

        // Amendment: contract and per-product actions
        public int? AmendedContractId { get; set; }

        // Audit fields
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        public List<ProposalProductDto> Products { get; set; } = new();
        public List<int> DistributorIds { get; set; } = new();
        public List<int> IndustryIds { get; set; } = new();
        public List<int> OpcoIds { get; set; } = new();
    }

    public class ProposalCreateDto
    {
        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public int ProposalTypeId { get; set; }
        [Required] public int ProposalStatusId { get; set; }
        public int? ManufacturerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? InternalNotes { get; set; }

        // Amendment: contract being amended (optional for non-amendment proposals)
        public int? AmendedContractId { get; set; }

        public List<ProposalProductCreateDto> Products { get; set; } = new();
        public List<int> DistributorIds { get; set; } = new();
        public List<int> IndustryIds { get; set; } = new();
        public List<int> OpcoIds { get; set; } = new();
    }

    public class ProposalUpdateDto : ProposalCreateDto { }

    public class ProposalProductDto
    {
        public int ProductId { get; set; }
        public int? PriceTypeId { get; set; }
        public int? Quantity { get; set; }
        public string? MetaJson { get; set; }
        public int? ProductProposalStatusId { get; set; }

        // Amendment: Add / Modify indicator
        public int? AmendmentActionId { get; set; }

        // Pricing fields (aligned with Contract pricing)
        public string? Uom { get; set; } // Cases | Pounds
        public bool BillbacksAllowed { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelPrice { get; set; }
        public decimal? CommercialFobPrice { get; set; }
        public decimal? CommodityDelPrice { get; set; }
        public decimal? CommodityFobPrice { get; set; }

        // Additional fields
        public decimal? Pua { get; set; }
        public decimal? FfsPrice { get; set; }
        public bool? NoiPrice { get; set; }
        public decimal? Ptv { get; set; }
        public string? InternalNotes { get; set; }
        public string? ManufacturerNotes { get; set; }
    }

    public class ProposalProductCreateDto : ProposalProductDto { }
}


    public class RejectProposalRequest
    {
        public string? RejectReason { get; set; }
    }

