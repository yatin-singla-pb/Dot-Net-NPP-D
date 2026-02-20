using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Domain.Proposals.Entities
{
    [Table("Proposals")]
    public class Proposal
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int ProposalTypeId { get; set; }
        public ProposalType? ProposalType { get; set; }

        [Required]
        public int ProposalStatusId { get; set; }
        public ProposalStatus? ProposalStatus { get; set; }

        public int? ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }

        // Amendment: the contract this proposal is amending (if any)
        public int? AmendedContractId { get; set; }
        public Contract? AmendedContract { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? DueDate { get; set; }

        [MaxLength(1000)]
        public string? InternalNotes { get; set; }

        [MaxLength(1000)]
        public string? RejectReason { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string CreatedBy { get; set; } = "System";
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)] public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ProposalProduct> Products { get; set; } = new List<ProposalProduct>();
        public ICollection<ProposalDistributor> Distributors { get; set; } = new List<ProposalDistributor>();
        public ICollection<ProposalIndustry> Industries { get; set; } = new List<ProposalIndustry>();
        public ICollection<ProposalOpco> Opcos { get; set; } = new List<ProposalOpco>();
        public ICollection<ProposalStatusHistory> StatusHistory { get; set; } = new List<ProposalStatusHistory>();
    }
}

