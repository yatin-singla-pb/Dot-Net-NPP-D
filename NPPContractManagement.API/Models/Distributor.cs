using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.Models
{
    public enum DistributorStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public class Distributor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

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

        public bool ReceiveContractProposal { get; set; } = true;

        /// <summary>
        /// Indicates if this distributor is a DOT/redistributor requiring special pricing handling
        /// </summary>
        public bool IsRedistributor { get; set; } = false;

        public DistributorStatus Status { get; set; } = DistributorStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<OpCo> OpCos { get; set; } = new List<OpCo>();
        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    }
}
