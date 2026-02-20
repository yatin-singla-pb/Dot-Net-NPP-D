using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class InitiateRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;
    }

    public class CheckUserIdDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserId { get; set; } = string.Empty;
    }

    public class CompleteRegistrationDto
    {
        [Required]
        public string RegistrationToken { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegistrationInitiatedResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime? CodeExpiresAt { get; set; }
    }

    public class VerifyCodeResponseDto
    {
        public string RegistrationToken { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class CheckUserIdResponseDto
    {
        public bool IsAvailable { get; set; }
        public string? Message { get; set; }
    }
}
