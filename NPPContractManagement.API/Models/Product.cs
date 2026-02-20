using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum ProductStatus
    {
        Active = 1,
        Inactive = 2
    }

    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ManufacturerProductCode { get; set; }

        [StringLength(100)]
        public string? GTIN { get; set; }

        [StringLength(100)]
        public string? UPC { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }

        [StringLength(100)]
        public string? PackSize { get; set; }

        [Required]
        public int ManufacturerId { get; set; }


        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(100)]
        public string? SubCategory { get; set; }


        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(100)]
        public string? TertiaryCategory { get; set; }

        public bool? AlwaysList { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }


        public ProductStatus Status { get; set; } = ProductStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; } = null!;

        public virtual ICollection<ContractProduct> ContractProducts { get; set; } = new List<ContractProduct>();
        public virtual ICollection<DistributorProductCode> DistributorProductCodes { get; set; } = new List<DistributorProductCode>();
    }
}
