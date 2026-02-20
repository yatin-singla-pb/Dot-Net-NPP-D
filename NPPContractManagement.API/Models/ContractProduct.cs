using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int ProductId { get; set; }

        // Track the current version this base row belongs to (align with other relationship tables)
        [Required]
        public int CurrentVersionNumber { get; set; } = 1;

        public int? MinimumQuantity { get; set; }

        public int? MaximumQuantity { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
