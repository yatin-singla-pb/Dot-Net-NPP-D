using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class RegistrationVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(64)]
        public string VerificationCodeHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public int AttemptCount { get; set; } = 0;

        public bool IsUsed { get; set; } = false;

        [StringLength(500)]
        public string? RegistrationToken { get; set; }

        public DateTime? RegistrationTokenExpiresAt { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
