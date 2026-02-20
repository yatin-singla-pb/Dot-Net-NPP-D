using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ManufacturerProductCode { get; set; }
        public string? GTIN { get; set; }
        public string? UPC { get; set; }
        public string? SKU { get; set; }
        public string? PackSize { get; set; }
        public int ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Brand { get; set; }
        public string? TertiaryCategory { get; set; }
        public bool? AlwaysList { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateProductDto
    {
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


        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateProductDto
    {
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


        public int Status { get; set; }

        public bool IsActive { get; set; }
    }
}
