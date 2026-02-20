using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Represents an ingested file (uploaded, SFTP, URL, or scheduled)
    /// </summary>
    [Table("IngestedFiles")]
    public class IngestedFile
    {
        [Key]
        [Column("file_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileId { get; set; }

        [Column("original_filename")]
        [MaxLength(512)]
        public string? OriginalFilename { get; set; }

        [Column("uploaded_by")]
        [MaxLength(255)]
        public string? UploadedBy { get; set; }

        [Column("source_type")]
        [MaxLength(20)]
        public string SourceType { get; set; } = "upload"; // upload, sftp, url, scheduler

        [Column("source_details", TypeName = "json")]
        public string? SourceDetails { get; set; } // JSON: {"sftp_host":"...","remote_path":"..."}

        [Column("content_sha256")]
        [MaxLength(128)]
        public string? ContentSha256 { get; set; }

        [Column("bytes")]
        public long? Bytes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<VelocityJob> VelocityJobs { get; set; } = new List<VelocityJob>();
        public virtual ICollection<VelocityShipment> VelocityShipments { get; set; } = new List<VelocityShipment>();
        public virtual ICollection<VelocityJobRow> VelocityJobRows { get; set; } = new List<VelocityJobRow>();
    }
}

