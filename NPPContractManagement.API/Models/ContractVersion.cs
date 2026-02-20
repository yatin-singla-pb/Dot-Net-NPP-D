using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public class ContractVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        // New schema fields to align with Contracts
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty; // Common description defined by the user

        public string? ForeignContractId { get; set; } // External/Entegra Contract ID

        public bool SendToPerformance { get; set; } = false;

        public bool IsSuspended { get; set; } = false;

        public DateTime? SuspendedDate { get; set; } // only populated if IsSuspended = true

        [Column(TypeName = "longtext")]
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata snapshot
        [StringLength(100)]
        public string? ManufacturerReferenceNumber { get; set; }

        [StringLength(200)]
        public string? ManufacturerBillbackName { get; set; }

        [Column(TypeName = "longtext")]
        public string? ManufacturerTermsAndConditions { get; set; }

        [StringLength(2000)]
        public string? ManufacturerNotes { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [StringLength(100)]
        public string? EntegraContractType { get; set; }

        [StringLength(100)]
        public string? EntegraVdaProgram { get; set; }


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [StringLength(100)]
        public string? AssignedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;


        [NotMapped]
        public virtual ICollection<ContractVersionPrice> Prices { get; set; } = new List<ContractVersionPrice>();

    }
}
