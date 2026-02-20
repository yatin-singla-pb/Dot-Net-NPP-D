using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Domain.Proposals.Entities
{
    [Table("ProposalStatusHistory")]
    public class ProposalStatusHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }
        [Required]
        public int FromStatusId { get; set; }
        public ProposalStatus? FromStatus { get; set; }
        [Required]
        public int ToStatusId { get; set; }
        public ProposalStatus? ToStatus { get; set; }
        [MaxLength(500)] public string? Comment { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string ChangedBy { get; set; } = "System";
    }

    [Table("ProposalProductHistory")]
    public class ProposalProductHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProposalProductId { get; set; }
        public ProposalProduct? ProposalProduct { get; set; }
        [MaxLength(50)] public string ChangeType { get; set; } = string.Empty; // Created/Updated/Deleted
        public string? PreviousJson { get; set; }
        public string? CurrentJson { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string ChangedBy { get; set; } = "System";
    }

    [Table("ProposalBatchJobs")]
    public class ProposalBatchJob
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)] public string JobType { get; set; } = string.Empty;
        [MaxLength(50)] public string Status { get; set; } = "Pending";
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Errors { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [MaxLength(100)] public string CreatedBy { get; set; } = "System";
    }
}

