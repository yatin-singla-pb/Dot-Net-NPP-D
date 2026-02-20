using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class MemberAccountDto
    {
        public int Id { get; set; }
        public string MemberNumber { get; set; } = string.Empty;
        public string FacilityName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }

        public int? IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? W9 { get; set; }
        public DateTime? W9Date { get; set; }
        public string? TaxId { get; set; }
        public string? BusinessType { get; set; }

        // New fields
        public DateTime? LopDate { get; set; }
        public string? InternalNotes { get; set; }
        public int? ClientGroupEnrollment { get; set; }
        public string SalesforceAccountName { get; set; } = string.Empty;
        public string? VMAPNumber { get; set; }
        public string? VMSupplierName { get; set; }
        public string? VMSupplierSite { get; set; }
        public string? PayType { get; set; }
        public string? PayTypeName { get; set; }
        public string? ParentMemberAccountNumber { get; set; }
        public string? EntegraGPONumber { get; set; }
        public string? ClientGroupNumber { get; set; }
        public string? EntegraIdNumber { get; set; }
        public DateTime? AuditDate { get; set; }

        public int Status { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public int CustomerAccountsCount { get; set; }
    }

    public class CreateMemberAccountDto
    {
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

        // New fields
        public DateTime? LopDate { get; set; }
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

        public string? PayType { get; set; }

        [StringLength(100)]
        public string? ParentMemberAccountNumber { get; set; }

        [StringLength(100)]
        public string? EntegraGPONumber { get; set; }

        [StringLength(100)]
        public string? ClientGroupNumber { get; set; }

        [StringLength(100)]
        public string? EntegraIdNumber { get; set; }

        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateMemberAccountDto
    {
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

        public string? W9 { get; set; }

        public DateTime? W9Date { get; set; }

        [StringLength(500)]
        public string? TaxId { get; set; }

        [StringLength(200)]
        public string? BusinessType { get; set; }

        // New fields
        public DateTime? LopDate { get; set; }
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

        public int Status { get; set; }

        public bool IsActive { get; set; }
    }
}
