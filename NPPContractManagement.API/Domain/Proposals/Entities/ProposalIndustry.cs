using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Domain.Proposals.Entities
{
    [Table("ProposalIndustries")]
    public class ProposalIndustry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }

        [Required]
        public int IndustryId { get; set; }
        public Industry? Industry { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string CreatedBy { get; set; } = "System";
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(100)] public string? ModifiedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

