using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractManufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        public int CurrentVersionNumber { get; set; } = 1;

        [StringLength(100)]
        public string? AssignedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; } = null!;
    }
}

