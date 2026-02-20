 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum CustomerAccountStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3,
        Suspended = 4,
        Closed = 5,
        Billing = 6,
        Test = 7,
        Prospect = 8,
        Rebate = 9,
        USDA = 10
    }

    public enum CustomerAssociation
    {
        CSN = 1,
        Combined = 2,
        RCDM = 3,
        SEMUPC = 4
    }

    public class CustomerAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MemberAccountId { get; set; }

        [Required]
        public int DistributorId { get; set; }

        public int? OpCoId { get; set; }

        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CustomerAccountNumber { get; set; } = string.Empty;

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


        // New fields
        [StringLength(200)]
        public string? SalesRep { get; set; }

        [StringLength(50)]
        public string? DSO { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool TRACSAccess { get; set; } = false;

        [StringLength(1)]
        [RegularExpression(@"^[$%]$", ErrorMessage = "Markup must be either $ or %.")]
        [Column(TypeName = "varchar(1)")]
        public string? Markup { get; set; }

        public DateTime? AuditDate { get; set; }

        public bool ToEntegra { get; set; } = false;

        public DateTime? DateToEntegra { get; set; }

        [StringLength(200)]
        public string? CombinedUniqueID { get; set; }

        public string? InternalNotes { get; set; }

        public CustomerAssociation Association { get; set; } = CustomerAssociation.CSN;

        public CustomerAccountStatus Status { get; set; } = CustomerAccountStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("MemberAccountId")]
        public virtual MemberAccount MemberAccount { get; set; } = null!;

        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; } = null!;

        [ForeignKey("OpCoId")]
        public virtual OpCo? OpCo { get; set; }
    }
}
