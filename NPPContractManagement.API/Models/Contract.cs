using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NPPContractManagement.API.Domain.Proposals.Entities;

namespace NPPContractManagement.API.Models
{
    public enum ContractStatus
    {
        Draft = 1,
        Pending = 2,
        Active = 3,
        Expired = 4,
        Terminated = 5,
        Suspended = 6
    }

    public class Contract
    {
        [Key]
        public int Id { get; set; }


        // New: User-defined common description
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        // Deprecated legacy fields (ignored by EF)
        [NotMapped]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [NotMapped]
        public int ManufacturerId { get; set; }


        // New: standardized external reference
        [StringLength(100)]
        public string? ForeignContractId { get; set; }

        // Legacy external reference (kept for compatibility in API DTOs only)
        [NotMapped]
        [StringLength(100)]
        public string? ForeignContractID { get; set; }

        public DateTime? SuspendedDate { get; set; }

        [NotMapped]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [NotMapped]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalValue { get; set; }

        // New: internal notes
        [StringLength(2000)]
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata
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


        // Legacy notes (ignored by EF)
        [NotMapped]
        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsSuspended { get; set; } = false;

        public bool SendToPerformance { get; set; } = false;

        public int CurrentVersionNumber { get; set; } = 1;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]


        public string? CreatedBy { get; set; }

        [StringLength(100)]

        // Link back to source Proposal (optional)
        public int? ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        public string? ModifiedBy { get; set; }

        // Navigation properties
        [NotMapped]
        public virtual Manufacturer Manufacturer { get; set; } = null!;

        public virtual ICollection<ContractProduct> ContractProducts { get; set; } = new List<ContractProduct>();
        public virtual ICollection<ContractVersion> ContractVersions { get; set; } = new List<ContractVersion>();
        public virtual ICollection<ContractDistributor> ContractDistributors { get; set; } = new List<ContractDistributor>();
        public virtual ICollection<ContractOpCo> ContractOpCos { get; set; } = new List<ContractOpCo>();
        public virtual ICollection<ContractIndustry> ContractIndustries { get; set; } = new List<ContractIndustry>();
        public virtual ICollection<ContractManufacturer> ContractManufacturers { get; set; } = new List<ContractManufacturer>();
        public virtual ICollection<ContractPrice> ContractPrices { get; set; } = new List<ContractPrice>();
    }
}
