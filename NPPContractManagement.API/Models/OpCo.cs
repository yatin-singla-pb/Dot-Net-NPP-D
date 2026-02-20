using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum OpCoStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public class OpCo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? RemoteReferenceCode { get; set; }

        [Required]
        public int DistributorId { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [StringLength(1000)]
        public string? InternalNotes { get; set; }

        /// <summary>
        /// Indicates if this OpCo is a DOT/redistributor requiring special pricing handling
        /// </summary>
        public bool IsRedistributor { get; set; } = false;

        public OpCoStatus Status { get; set; } = OpCoStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; } = null!;

        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
        public virtual ICollection<ContractOpCo> ContractOpCos { get; set; } = new List<ContractOpCo>();
    }
}
