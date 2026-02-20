using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Represents aggregated job errors for velocity imports
    /// </summary>
    [Table("VelocityErrors")]
    public class VelocityError
    {
        [Key]
        [Column("error_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorId { get; set; }

        [Column("job_id")]
        public int JobId { get; set; }

        [Column("error_code")]
        [MaxLength(64)]
        public string? ErrorCode { get; set; }

        [Column("message", TypeName = "text")]
        public string? Message { get; set; }

        [Column("details", TypeName = "json")]
        public string? Details { get; set; } // JSON with additional error details

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(JobId))]
        public virtual VelocityJob? VelocityJob { get; set; }
    }
}

