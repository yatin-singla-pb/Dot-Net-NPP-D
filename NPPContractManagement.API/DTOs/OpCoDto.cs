using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class OpCoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? RemoteReferenceCode { get; set; }
        public int DistributorId { get; set; }
        public string? DistributorName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? InternalNotes { get; set; }
        public bool IsRedistributor { get; set; }
        public int Status { get; set; }
        public int CustomerAccountsCount { get; set; }
        public List<CustomerAccountSummaryDto>? CustomerAccounts { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateOpCoDto
    {
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

        public bool IsRedistributor { get; set; } = false;

        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateOpCoDto
    {
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

        public bool IsRedistributor { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }
    }

    public class CustomerAccountSummaryDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAccountNumber { get; set; } = string.Empty;
        public string? StatusName { get; set; }
    }
}
