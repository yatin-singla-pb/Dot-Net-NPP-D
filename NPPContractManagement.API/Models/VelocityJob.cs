using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Represents a velocity data import job
    /// </summary>
    [Table("VelocityJobs")]
    public class VelocityJob
    {
        [Key]
        [Column("job_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("file_id")]
        public int? FileId { get; set; }

        [Column("distributor_id")]
        public int? DistributorId { get; set; }

        [Column("initiated_by")]
        [MaxLength(255)]
        public string? InitiatedBy { get; set; }

        [Column("started_at")]
        public DateTime? StartedAt { get; set; }

        [Column("finished_at")]
        public DateTime? FinishedAt { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "queued";

        [Column("totals", TypeName = "json")]
        public string? TotalsJson { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Computed properties for backward compatibility
        [NotMapped]
        public string JobId => Id.ToString();

        [NotMapped]
        public string? CreatedBy
        {
            get => InitiatedBy;
            set => InitiatedBy = value;
        }

        [NotMapped]
        public DateTime? CompletedAt
        {
            get => FinishedAt;
            set => FinishedAt = value;
        }

        [NotMapped]
        public string? FileName { get; set; }

        [NotMapped]
        public string? SftpFileUrl { get; set; }

        [NotMapped]
        public int TotalRows
        {
            get => GetTotalValue("rows_total");
            set => SetTotalValue("rows_total", value);
        }

        [NotMapped]
        public int ProcessedRows
        {
            get => TotalRows;
            set { }
        }

        [NotMapped]
        public int SuccessRows
        {
            get => GetTotalValue("rows_success");
            set => SetTotalValue("rows_success", value);
        }

        [NotMapped]
        public int FailedRows
        {
            get => GetTotalValue("rows_failed");
            set => SetTotalValue("rows_failed", value);
        }

        [NotMapped]
        public string? ErrorMessage { get; set; }

        private int GetTotalValue(string key)
        {
            if (string.IsNullOrEmpty(TotalsJson)) return 0;
            try
            {
                var totals = JsonSerializer.Deserialize<Dictionary<string, int>>(TotalsJson);
                return totals?.ContainsKey(key) == true ? totals[key] : 0;
            }
            catch { return 0; }
        }

        private void SetTotalValue(string key, int value)
        {
            try
            {
                var totals = string.IsNullOrEmpty(TotalsJson)
                    ? new Dictionary<string, int>()
                    : JsonSerializer.Deserialize<Dictionary<string, int>>(TotalsJson) ?? new Dictionary<string, int>();

                totals[key] = value;
                TotalsJson = JsonSerializer.Serialize(totals);
            }
            catch { }
        }

        // Navigation properties
        [ForeignKey(nameof(FileId))]
        public virtual IngestedFile? IngestedFile { get; set; }

        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor? Distributor { get; set; }

        public virtual ICollection<VelocityJobRow> JobRows { get; set; } = new List<VelocityJobRow>();
        public virtual ICollection<VelocityShipment> Shipments { get; set; } = new List<VelocityShipment>();
        public virtual ICollection<VelocityError> Errors { get; set; } = new List<VelocityError>();
    }

    public enum VelocityJobStatus
    {
        Queued,
        Processing,
        Completed,
        Failed,
        PartialSuccess,
        CompletedWithErrors
    }
}

