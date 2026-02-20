using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Domain.Proposals.Entities
{
    [Table("ProposalStatuses")]
    public class ProposalStatus
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    [Table("ProposalTypes")]
    public class ProposalType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    [Table("PriceTypes")]
    public class PriceType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    [Table("ProductProposalStatuses")]
    public class ProductProposalStatus
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    [Table("AmendmentActions")]
    public class AmendmentAction
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

