using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractVersionPrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int PriceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string PriceType { get; set; } = string.Empty;

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

        [Required]
        [StringLength(50)]
        public string UOM { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,4)")]
        public decimal? EstimatedQty { get; set; }

        public bool BillbacksAllowed { get; set; } = false;

        [Column(TypeName = "decimal(18,4)")]
        public decimal? PUA { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? FFSPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? NOIPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? PTV { get; set; }

        [Column(TypeName = "longtext")]
        public string? InternalNotes { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        [StringLength(100)]
        public string? AssignedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        [ForeignKey("PriceId")]
        public virtual ContractPrice ContractPrice { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [NotMapped]
        public decimal? Price { get; set; }

        [NotMapped]
        public string? Tier { get; set; }

        [NotMapped]
        public DateTime? EffectiveFrom { get; set; }

        [NotMapped]
        public DateTime? EffectiveTo { get; set; }
    }
}

