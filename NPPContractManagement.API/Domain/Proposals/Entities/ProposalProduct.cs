using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Domain.Proposals.Entities
{
    [Table("ProposalProducts")]
    public class ProposalProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int? PriceTypeId { get; set; }
        public PriceType? PriceType { get; set; }

        public int? Quantity { get; set; }

        // Amendment action (Add / Modify) when this proposal is an amendment
        public int? AmendmentActionId { get; set; }
        public AmendmentAction? AmendmentAction { get; set; }

        // New pricing fields (aligned with Contract pricing)
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Allowance { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CommercialDelPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CommercialFobPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CommodityDelPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CommodityFobPrice { get; set; }

        [MaxLength(20)]
        public string? Uom { get; set; } // Cases | Pounds

        public bool BillbacksAllowed { get; set; }

        // Additional fields already present
        [Column(TypeName = "decimal(18,4)")]
        public decimal? Pua { get; set; } // Pickup Allowance

        [Column(TypeName = "decimal(18,4)")]
        public decimal? FfsPrice { get; set; }

        public bool? NoiPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Ptv { get; set; }

        [MaxLength(1000)]
        public string? InternalNotes { get; set; }

        [MaxLength(1000)]
        public string? ManufacturerNotes { get; set; }

        // JSON string (kept for backward compatibility; may be unused now)
        public string? MetaJson { get; set; }

        public int? ProductProposalStatusId { get; set; }
        public ProductProposalStatus? ProductProposalStatus { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string CreatedBy { get; set; } = "System";
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)] public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ProposalProductHistory> History { get; set; } = new List<ProposalProductHistory>();
    }
}

