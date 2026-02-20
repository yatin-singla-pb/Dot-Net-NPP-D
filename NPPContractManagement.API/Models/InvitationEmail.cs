using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class InvitationEmail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool WasSent { get; set; } = false;

        [StringLength(1000)]
        public string? FailureReason { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
