using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Represents a distributor shipment record from velocity data (canonical valid rows)
    /// </summary>
    [Table("VelocityShipments")]
    public class VelocityShipment
    {
        [Key]
        [Column("shipment_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("distributor_id")]
        [MaxLength(128)]
        public string? DistributorIdStr { get; set; }

        [Column("sku")]
        [MaxLength(255)]
        public string? Sku { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("shipped_at")]
        public DateTime? ShippedAt { get; set; }

        [Column("origin")]
        [MaxLength(255)]
        public string? Origin { get; set; }

        [Column("destination")]
        [MaxLength(255)]
        public string? Destination { get; set; }

        [Column("manifest_line", TypeName = "json")]
        public string? ManifestLine { get; set; } // JSON of all row data

        [Column("ingested_at")]
        public DateTime IngestedAt { get; set; } = DateTime.UtcNow;

        [Column("job_id")]
        public int? JobId { get; set; }

        [Column("file_id")]
        public int? FileId { get; set; }

        // Backward compatibility properties
        [NotMapped]
        public string ShipmentId
        {
            get => Id.ToString();
            set { }
        }

        [NotMapped]
        public int DistributorId { get; set; }

        [NotMapped]
        public int? VelocityJobId
        {
            get => JobId;
            set => JobId = value;
        }

        [NotMapped]
        public int? RowIndex { get; set; }

        [NotMapped]
        public DateTime CreatedAt
        {
            get => IngestedAt;
            set => IngestedAt = value;
        }

        [NotMapped]
        public string? CreatedBy { get; set; }

        [NotMapped]
        public virtual Distributor? Distributor { get; set; }

        // Navigation properties
        [ForeignKey(nameof(JobId))]
        public virtual VelocityJob? VelocityJob { get; set; }

        [ForeignKey(nameof(FileId))]
        public virtual IngestedFile? IngestedFile { get; set; }
    }
}

