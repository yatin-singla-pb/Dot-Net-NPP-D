using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Represents the processing result of a single row in a velocity import job (row-level audit and errors)
    /// </summary>
    [Table("VelocityJobRows")]
    public class VelocityJobRow
    {
        [Key]
        [Column("row_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("job_id")]
        public int JobId { get; set; }

        [Column("file_id")]
        public int FileId { get; set; }

        [Column("row_index")]
        public int RowIndex { get; set; } // 0-based index in CSV

        [Column("raw_values", TypeName = "json")]
        public string? RawValues { get; set; } // JSON of the row data

        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, success, failed, skipped

        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Exception action tracking
        [Column("action_status")]
        [MaxLength(30)]
        public string? ActionStatus { get; set; } // null=open, Dismissed, NewContract, Amendment

        [Column("action_notes", TypeName = "text")]
        public string? ActionNotes { get; set; }

        [Column("action_taken_by")]
        [MaxLength(100)]
        public string? ActionTakenBy { get; set; }

        [Column("action_taken_at")]
        public DateTime? ActionTakenAt { get; set; }

        // Backward compatibility properties
        [NotMapped]
        public int VelocityJobId
        {
            get => JobId;
            set => JobId = value;
        }

        [NotMapped]
        public string? RawData
        {
            get => RawValues;
            set => RawValues = value;
        }

        [NotMapped]
        public DateTime ProcessedAt
        {
            get => CreatedAt;
            set => CreatedAt = value;
        }

        [NotMapped]
        public int? VelocityShipmentId { get; set; }

        [NotMapped]
        public virtual VelocityShipment? VelocityShipment { get; set; }

        // Navigation properties
        [ForeignKey(nameof(JobId))]
        public virtual VelocityJob? VelocityJob { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual IngestedFile? IngestedFile { get; set; }
    }
}

