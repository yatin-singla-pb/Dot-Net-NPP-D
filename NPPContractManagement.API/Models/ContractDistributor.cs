using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractDistributor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int DistributorId { get; set; }

        public int CurrentVersionNumber { get; set; } = 1;

        [StringLength(100)]
        public string? AssignedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        // Legacy fields (preserved)
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; } = null!;
    }
}
