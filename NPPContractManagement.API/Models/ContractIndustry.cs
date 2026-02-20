using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractIndustry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int IndustryId { get; set; }

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

        [ForeignKey("IndustryId")]
        public virtual Industry Industry { get; set; } = null!;
    }
}
