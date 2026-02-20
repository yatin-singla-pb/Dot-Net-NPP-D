using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class ManufacturerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AKA { get; set; }
        public string? Description { get; set; }
        public string? ContactPerson { get; set; }
        public int? ContactPersonId { get; set; }
        public string? ContactPersonName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public int? PrimaryBrokerId { get; set; }
        public string? PrimaryBrokerName { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateManufacturerDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AKA { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        public int? ContactPersonId { get; set; }

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

        public int? PrimaryBrokerId { get; set; }

        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateManufacturerDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AKA { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        public int? ContactPersonId { get; set; }

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

        public int? PrimaryBrokerId { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }
    }
}
