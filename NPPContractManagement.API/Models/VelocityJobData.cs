using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Stores the parsed CSV data for a velocity job so it can be resumed after restart
    /// </summary>
    [Table("VelocityJobData")]
    public class VelocityJobData
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to VelocityJob
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Serialized validation results (JSON array)
        /// </summary>
        [Column(TypeName = "longtext")]
        public string ValidationResultsJson { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user who created the job (for notifications)
        /// </summary>
        [MaxLength(255)]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// When this data was stored
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property
        /// </summary>
        [ForeignKey("JobId")]
        public virtual VelocityJob? Job { get; set; }
    }
}

