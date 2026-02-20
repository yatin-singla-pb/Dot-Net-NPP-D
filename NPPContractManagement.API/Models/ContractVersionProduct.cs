using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    [Table("ContractVersionProduct")]
    public class ContractVersionProduct
    {
        [Key] public int Id { get; set; }
        [Required] public int ContractId { get; set; }
        [Required] public int ProductId { get; set; }
        [Required] public int VersionNumber { get; set; }
        [StringLength(100)] public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }

        [ForeignKey("ContractId")] public virtual Contract Contract { get; set; } = null!;
        [ForeignKey("ProductId")] public virtual Product Product { get; set; } = null!;
    }
}

