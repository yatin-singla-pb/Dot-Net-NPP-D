using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum MemberAccountStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3,
        Suspended = 4
    }

    public enum PayType
    {
        ACH = 1,
        Check = 2,
        Wire = 3
    }

    public class MemberAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string MemberNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string FacilityName { get; set; } = string.Empty;

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



        public int? IndustryId { get; set; }

        [StringLength(50)]
        public string? W9 { get; set; }

        public DateTime? W9Date { get; set; }

        [StringLength(500)]
        public string? TaxId { get; set; }

        [StringLength(200)]
        public string? BusinessType { get; set; }

        // New extended fields
        public DateTime? LopDate { get; set; }

        [Column(TypeName = "text")]
        public string? InternalNotes { get; set; }

        [Range(0, int.MaxValue)]
        public int? ClientGroupEnrollment { get; set; }

        [Required]
        [StringLength(200)]
        public string SalesforceAccountName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? VMAPNumber { get; set; }

        [StringLength(200)]
        public string? VMSupplierName { get; set; }

        [StringLength(200)]
        public string? VMSupplierSite { get; set; }

        [StringLength(50)]
        public string? PayType { get; set; }

        [StringLength(100)]
        public string? ParentMemberAccountNumber { get; set; }

        [StringLength(100)]
        public string? EntegraGPONumber { get; set; }

        [StringLength(100)]
        public string? ClientGroupNumber { get; set; }

        [StringLength(100)]
        public string? EntegraIdNumber { get; set; }

        public DateTime? AuditDate { get; set; }

        public MemberAccountStatus Status { get; set; } = MemberAccountStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        [ForeignKey("IndustryId")]
        public virtual Industry? Industry { get; set; }

        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    }
}
