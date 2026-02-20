using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractPrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        [StringLength(100)]
        public string PriceType { get; set; } = string.Empty; // "Contract Price", "List at time of purchase / No Bid", "Product Suspended"

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
        public string UOM { get; set; } = string.Empty; // "Cases" or "Pounds"

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

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        [StringLength(100)] public string? CreatedBy { get; set; }
        [StringLength(100)] public string? ModifiedBy { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;
    }
}

