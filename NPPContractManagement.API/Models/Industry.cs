using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.Models
{
    public enum IndustryStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public class Industry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public IndustryStatus Status { get; set; } = IndustryStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<MemberAccount> MemberAccounts { get; set; } = new List<MemberAccount>();
        public virtual ICollection<ContractIndustry> ContractIndustries { get; set; } = new List<ContractIndustry>();
    }
}
