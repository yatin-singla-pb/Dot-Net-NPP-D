using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public string? Notes { get; set; }

        public int? IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public bool GroupEmail { get; set; }
        public int AccountStatus { get; set; }
        public string? AccountStatusName { get; set; }
        public int Status { get; set; }

        public string? Class { get; set; }
        public bool IsActive { get; set; }
        public bool IsHeadless { get; set; }
        public bool EmailConfirmed { get; set; }
        public int FailedAuthAttempts { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }

    public class CreateUserDto
    {
        [StringLength(100)]
        public string? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; } = 1;
        public int FailedAuthAttempts { get; set; } = 0;


        public int? IndustryId { get; set; }

        public bool GroupEmail { get; set; } = false;

        public int AccountStatus { get; set; } = 1; // Active by default

        [StringLength(100)]
        public string? Class { get; set; }

        [Required]
        public List<int> RoleIds { get; set; } = new List<int>();

        public List<int>? ManufacturerIds { get; set; } = new List<int>();
    }

    public class UpdateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public string? Notes { get; set; }
        public int Status { get; set; }
        public int FailedAuthAttempts { get; set; }


        public int? IndustryId { get; set; }

        public bool GroupEmail { get; set; }

        public int AccountStatus { get; set; }

        [StringLength(100)]
        public string? Class { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public List<int> RoleIds { get; set; } = new List<int>();

        public List<int>? ManufacturerIds { get; set; } = new List<int>();
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class SetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
    }
}
