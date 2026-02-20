using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegistrationService> _logger;

        private static readonly HashSet<string> ReservedUserIds = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin", "administrator", "system", "root", "superadmin", "npp", "support"
        };

        public RegistrationService(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<RegistrationService> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<RegistrationInitiatedResponseDto> InitiateRegistrationAsync(string email)
        {
            // Always return success to prevent email enumeration
            var successResponse = new RegistrationInitiatedResponseDto
            {
                Message = "If your email is registered, you will receive a verification code shortly.",
                CodeExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };

            try
            {
                _logger.LogInformation("Registration initiate: looking up email {Email}", email);
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogInformation("Registration initiate: no user found for email {Email}", email);
                    return successResponse;
                }
                if (!user.IsHeadless || user.AccountStatus != AccountStatus.Headless)
                {
                    _logger.LogInformation("Registration initiate: user {Id} found but not headless (IsHeadless={IsHeadless}, AccountStatus={Status})", user.Id, user.IsHeadless, user.AccountStatus);
                    return successResponse;
                }

                // Rate limit: max 3 codes per hour
                var oneHourAgo = DateTime.UtcNow.AddHours(-1);
                var recentCount = await _context.RegistrationVerifications
                    .CountAsync(rv => rv.UserId == user.Id && rv.CreatedAt >= oneHourAgo);

                if (recentCount >= 3)
                {
                    _logger.LogWarning("Registration initiate: rate limit exceeded for user {Id}", user.Id);
                    return successResponse;
                }

                // Generate 6-digit code
                var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
                var codeHash = HashWithSha256(code);

                var verification = new RegistrationVerification
                {
                    UserId = user.Id,
                    VerificationCodeHash = codeHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    AttemptCount = 0,
                    IsUsed = false
                };

                _context.RegistrationVerifications.Add(verification);
                await _context.SaveChangesAsync();

                // Send verification code email
                var emailSent = await _emailService.SendVerificationCodeEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    code);

                if (emailSent)
                {
                    _logger.LogInformation("Verification code sent for user {Id}", user.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to send verification code email for user {Id} to {Email}", user.Id, user.Email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration initiation");
            }

            return successResponse;
        }

        public async Task<VerifyCodeResponseDto?> VerifyCodeAsync(string email, string code)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsHeadless)
            {
                return null;
            }

            // Find the latest non-used, non-expired verification
            var verification = await _context.RegistrationVerifications
                .Where(rv => rv.UserId == user.Id && !rv.IsUsed && rv.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rv => rv.CreatedAt)
                .FirstOrDefaultAsync();

            if (verification == null)
            {
                return null;
            }

            // Increment attempt count
            verification.AttemptCount++;

            if (verification.AttemptCount > 5)
            {
                verification.IsUsed = true;
                await _context.SaveChangesAsync();
                _logger.LogWarning("Max attempts exceeded for verification {Id}", verification.Id);
                return null;
            }

            // Compare hashes
            var codeHash = HashWithSha256(code);
            if (codeHash != verification.VerificationCodeHash)
            {
                await _context.SaveChangesAsync();
                return null;
            }

            // Code is valid - generate registration token
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var registrationToken = Convert.ToBase64String(tokenBytes);
            var tokenHash = HashWithSha256(registrationToken);

            verification.IsUsed = true;
            verification.RegistrationToken = tokenHash;
            verification.RegistrationTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Code verified successfully for user {Id}", user.Id);

            return new VerifyCodeResponseDto
            {
                RegistrationToken = registrationToken,
                TokenExpiresAt = verification.RegistrationTokenExpiresAt ?? DateTime.UtcNow.AddMinutes(30),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<CheckUserIdResponseDto> CheckUserIdAvailabilityAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || userId.Length < 3)
            {
                return new CheckUserIdResponseDto
                {
                    IsAvailable = false,
                    Message = "User ID must be at least 3 characters."
                };
            }

            if (ReservedUserIds.Contains(userId))
            {
                return new CheckUserIdResponseDto
                {
                    IsAvailable = false,
                    Message = "This User ID is reserved."
                };
            }

            var isTaken = await _userRepository.IsUserIdTakenAsync(userId);
            return new CheckUserIdResponseDto
            {
                IsAvailable = !isTaken,
                Message = isTaken ? "This User ID is already taken." : null
            };
        }

        public async Task<bool> CompleteRegistrationAsync(CompleteRegistrationDto dto)
        {
            var tokenHash = HashWithSha256(dto.RegistrationToken);

            // Find valid verification by token hash
            var verification = await _context.RegistrationVerifications
                .Where(rv => rv.RegistrationToken == tokenHash
                    && rv.RegistrationTokenExpiresAt > DateTime.UtcNow
                    && rv.IsUsed)
                .OrderByDescending(rv => rv.CreatedAt)
                .FirstOrDefaultAsync();

            if (verification == null)
            {
                _logger.LogWarning("Complete registration: invalid or expired token");
                return false;
            }

            var user = await _userRepository.GetByIdAsync(verification.UserId);
            if (user == null || !user.IsHeadless)
            {
                _logger.LogWarning("Complete registration: user not found or not headless");
                return false;
            }

            // Check UserId availability
            if (ReservedUserIds.Contains(dto.UserId))
            {
                _logger.LogWarning("Complete registration: reserved UserId attempted");
                return false;
            }

            if (await _userRepository.IsUserIdTakenAsync(dto.UserId))
            {
                _logger.LogWarning("Complete registration: UserId already taken");
                return false;
            }

            // Complete the registration
            user.UserId = dto.UserId;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsHeadless = false;
            user.AccountStatus = AccountStatus.Active;
            user.EmailConfirmed = true;
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifiedBy = "Registration";

            // Invalidate the token by clearing it
            verification.RegistrationToken = null;
            verification.RegistrationTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            // Send welcome email
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email during registration completion");
            }

            _logger.LogInformation("Registration completed for user {Id}, UserId={UserId}", user.Id, dto.UserId);
            return true;
        }

        public async Task<bool> ResendInvitationAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsHeadless)
            {
                return false;
            }

            var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var registrationLink = $"{frontendUrl}/register";

            var sent = await _emailService.SendRegistrationInvitationEmailAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                registrationLink);

            // Log invitation
            var invitation = new InvitationEmail
            {
                UserId = user.Id,
                SentAt = DateTime.UtcNow,
                WasSent = sent,
                FailureReason = sent ? null : "Email service returned false"
            };
            _context.InvitationEmails.Add(invitation);
            await _context.SaveChangesAsync();

            return sent;
        }

        private static string HashWithSha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
