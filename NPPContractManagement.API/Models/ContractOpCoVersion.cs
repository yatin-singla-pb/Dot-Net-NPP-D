using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    [Table("ContractOpCosVersion")]
    public class ContractOpCoVersion
    {
        [Key] public int Id { get; set; }
        [Required] public int ContractId { get; set; }
        [Required] public int OpCoId { get; set; }
        [Required] public int VersionNumber { get; set; }
        [StringLength(100)] public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }

        [ForeignKey("ContractId")] public virtual Contract Contract { get; set; } = null!;
        [ForeignKey("OpCoId")] public virtual OpCo OpCo { get; set; } = null!;
    }
}

