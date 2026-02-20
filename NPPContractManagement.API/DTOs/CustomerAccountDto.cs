using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class CustomerAccountDto
    {
        public int Id { get; set; }
        public int MemberAccountId { get; set; }
        public string? MemberAccountName { get; set; }
        public string? MemberNumber { get; set; }
        public int DistributorId { get; set; }
        public string? DistributorName { get; set; }
        public int? OpCoId { get; set; }
        public string? OpCoName { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAccountNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        // New fields
        public string? SalesRep { get; set; }
        public string? DSO { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool TRACSAccess { get; set; }
        public string? Markup { get; set; }
        public DateTime? AuditDate { get; set; }
        public bool ToEntegra { get; set; }
        public DateTime? DateToEntegra { get; set; }
        public string? CombinedUniqueID { get; set; }
        public string? InternalNotes { get; set; }
        public int Association { get; set; }
        public string? AssociationName { get; set; }

        public int Status { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateCustomerAccountDto
    {
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
        public string? DSO { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public bool TRACSAccess { get; set; }
        [StringLength(1)]
        [RegularExpression(@"^[$%]$", ErrorMessage = "Markup must be either $ or %.")]
        public string? Markup { get; set; }
        public DateTime? AuditDate { get; set; }
        public bool ToEntegra { get; set; }
        public DateTime? DateToEntegra { get; set; }
        [StringLength(200)]
        public string? CombinedUniqueID { get; set; }
        public string? InternalNotes { get; set; }
        public int? Association { get; set; }

        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateCustomerAccountDto
    {
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
        public string? DSO { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public bool TRACSAccess { get; set; }
        [StringLength(1)]
        [RegularExpression(@"^[$%]$", ErrorMessage = "Markup must be either $ or %.")]
        public string? Markup { get; set; }
        public DateTime? AuditDate { get; set; }
        public bool ToEntegra { get; set; }
        public DateTime? DateToEntegra { get; set; }
        [StringLength(200)]
        public string? CombinedUniqueID { get; set; }
        public string? InternalNotes { get; set; }
        public int? Association { get; set; }

        public int Status { get; set; }

        public bool? IsActive { get; set; }
    }
}
