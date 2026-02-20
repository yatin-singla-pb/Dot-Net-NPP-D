using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPPContractManagement.API.Models
{
    public enum ManufacturerStatus
    {
        Active = 1,
        Inactive = 2
    }

    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AKA { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        // Contact Person relationship (optional) - references a User associated with this manufacturer
        public int? ContactPersonId { get; set; }

        [ForeignKey(nameof(ContactPersonId))]
        public virtual User? ContactPersonUser { get; set; }

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

        // Primary Broker relationship (optional)
        public int? PrimaryBrokerId { get; set; }

        [ForeignKey(nameof(PrimaryBrokerId))]
        public virtual User? PrimaryBroker { get; set; }

        public ManufacturerStatus Status { get; set; } = ManufacturerStatus.Active;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
