using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum AccountStatus
    {
        Active = 1,
        Locked = 2,
        Suspended = 3,
        Headless = 4
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostCode { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }


        public int? IndustryId { get; set; }

        public bool GroupEmail { get; set; } = false;

        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;

        [Required]
        public int Status { get; set; } = 1;


        [StringLength(100)]
        public string? Class { get; set; }

        public string? PasswordHash { get; set; }

        public bool IsHeadless { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public bool EmailConfirmed { get; set; } = false;

        public int FailedAuthAttempts { get; set; } = 0;

        [StringLength(500)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("IndustryId")]
        public virtual Industry? Industry { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
