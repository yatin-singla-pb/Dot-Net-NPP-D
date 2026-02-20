using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Models;
using BCrypt.Net;

namespace NPPContractManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IUserManufacturerRepository _userManufacturerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IUserManufacturerRepository userManufacturerRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _userManufacturerRepository = userManufacturerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {UserId}", loginDto.UserId);

                var user = await _userRepository.GetUserWithRolesByUserIdAsync(loginDto.UserId);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: User not found", loginDto.UserId);
                    return null;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: User is inactive", loginDto.UserId);
                    return null;
                }

                // Block locked accounts
                if (user.AccountStatus == AccountStatus.Locked)
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: Account is locked", loginDto.UserId);
                    return null;
                }

                // Block suspended accounts
                if (user.AccountStatus == AccountStatus.Suspended)
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: Account is suspended", loginDto.UserId);
                    return null;
                }

                // Block headless accounts from logging in
                if (user.IsHeadless || user.AccountStatus == AccountStatus.Headless)
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: Account is headless (pending registration)", loginDto.UserId);
                    return null;
                }

                // Null-check PasswordHash before verification
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogWarning("Login attempt failed for user {UserId}: No password set", loginDto.UserId);
                    return null;
                }

                _logger.LogInformation("User found: {UserId}, checking password", loginDto.UserId);

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    // Increment failed auth attempts and lock after 5 failures
                    user.FailedAuthAttempts++;
                    if (user.FailedAuthAttempts >= 5)
                    {
                        user.AccountStatus = AccountStatus.Locked;
                        _logger.LogWarning("User {UserId} locked after {Attempts} failed login attempts", loginDto.UserId, user.FailedAuthAttempts);
                    }
                    user.ModifiedDate = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);

                    _logger.LogWarning("Login attempt failed for user {UserId}: Invalid password (attempt {Attempts}/5)", loginDto.UserId, user.FailedAuthAttempts);
                    return null;
                }

                // Successful login: reset failed attempts and update last login date
                user.FailedAuthAttempts = 0;
                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var userDto = MapUserToDto(user);
                // Include manufacturer_ids claim
                var manufacturerIds = await _userManufacturerRepository.GetManufacturerIdsForUserAsync(userDto.Id);
                var extraClaims = new List<Claim>();
                try
                {
                    var idsJson = System.Text.Json.JsonSerializer.Serialize(manufacturerIds);
                    extraClaims.Add(new Claim("manufacturer_ids", idsJson));
                }
                catch { }
                var token = GenerateJwtTokenWithExtras(userDto, extraClaims);
                var refreshToken = GenerateRefreshToken();

                // Persist refresh token on user for validation/invalidation
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User {UserId} logged in successfully", loginDto.UserId);

                return new LoginResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UserId}", loginDto.UserId);
                return null;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // Simplified refresh: read user identity from Authorization header (may be expired),
            // then issue fresh access/refresh tokens with current role and manufacturer_ids claims.
            try
            {
                _logger.LogInformation("Refresh token request received");
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return null;
                }

                var authHeader = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"].FirstOrDefault();
                var bearerToken = string.Empty;
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    bearerToken = authHeader.Substring("Bearer ".Length).Trim();
                }

                int userId;
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    try
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwt = tokenHandler.ReadJwtToken(bearerToken);
                        var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");
                        if (idClaim == null || !int.TryParse(idClaim.Value, out userId))
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    // As a fallback, reject if no current token is available to identify the user
                    return null;
                }

                var user = await _userRepository.GetUserWithRolesAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                // Validate stored refresh token matches and hasn't expired
                if (user.RefreshToken != refreshToken)
                {
                    _logger.LogWarning("Refresh token mismatch for user {UserId}", userId);
                    return null;
                }
                if (user.RefreshTokenExpiryTime.HasValue && user.RefreshTokenExpiryTime.Value < DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token expired for user {UserId}", userId);
                    return null;
                }

                var userDto = MapUserToDto(user);
                var manufacturerIds = await _userManufacturerRepository.GetManufacturerIdsForUserAsync(userDto.Id);
                var extraClaims = new List<Claim>();
                try
                {
                    var idsJson = System.Text.Json.JsonSerializer.Serialize(manufacturerIds);
                    extraClaims.Add(new Claim("manufacturer_ids", idsJson));
                }
                catch { }

                var newAccessToken = GenerateJwtTokenWithExtras(userDto, extraClaims);
                var newRefreshToken = GenerateRefreshToken();

                // Persist new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateAsync(user);

                return new LoginResponseDto
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return null;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userRepository.GetByUserIdAsync(forgotPasswordDto.UserId);
                if (user == null || !user.IsActive)
                {
                    // Don't reveal if user exists or not for security
                    return true;
                }

                var resetToken = await GeneratePasswordResetTokenAsync(user.Email);
                var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4201";
                var resetLink = $"{frontendUrl}/reset-password?token={resetToken}&email={user.Email}";

                await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    resetLink);

                _logger.LogInformation("Password reset email sent for user {UserId}", forgotPasswordDto.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email for user {UserId}", forgotPasswordDto.UserId);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!await ValidatePasswordResetTokenAsync(resetPasswordDto.Token, resetPasswordDto.Email))
                {
                    return false;
                }

                var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
                if (user == null || !user.IsActive)
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
                user.ModifiedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password reset successfully for user {Email}", resetPasswordDto.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", resetPasswordDto.Email);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                user.ModifiedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password changed successfully for user {UserId}", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task LogoutAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _userRepository.UpdateAsync(user);
                    _logger.LogInformation("User {UserId} logged out, refresh token invalidated", user.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            // In production, store this token in database with expiry
            var tokenData = $"{email}:{DateTime.UtcNow.AddHours(1):yyyy-MM-dd HH:mm:ss}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string token, string email)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(token);
                var tokenData = Encoding.UTF8.GetString(tokenBytes);
                // Split on first ':' only, since the datetime portion contains colons (HH:mm:ss)
                var separatorIndex = tokenData.IndexOf(':');
                if (separatorIndex < 0)
                {
                    return false;
                }

                var tokenEmail = tokenData.Substring(0, separatorIndex);
                var tokenExpiry = tokenData.Substring(separatorIndex + 1);

                if (tokenEmail != email)
                {
                    return false;
                }

                if (DateTime.TryParse(tokenExpiry, out var expiry))
                {
                    return DateTime.UtcNow <= expiry;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserId ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            // Add role claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtTokenWithExtras(UserDto user, IEnumerable<System.Security.Claims.Claim>? extraClaims)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!");

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserId ?? string.Empty),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim("firstName", user.FirstName),
                new System.Security.Claims.Claim("lastName", user.LastName)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role.Name));
            }

            if (extraClaims != null)
            {
                claims.AddRange(extraClaims);
            }

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = System.DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsHeadless = user.IsHeadless,
                EmailConfirmed = user.EmailConfirmed,
                LastLoginDate = user.LastLoginDate,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                CreatedBy = user.CreatedBy,
                ModifiedBy = user.ModifiedBy,
                Roles = user.UserRoles.Select(ur => new RoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description,
                    IsActive = ur.Role.IsActive,
                    CreatedDate = ur.Role.CreatedDate,
                    ModifiedDate = ur.Role.ModifiedDate,
                    CreatedBy = ur.Role.CreatedBy,
                    ModifiedBy = ur.Role.ModifiedBy
                }).ToList()
            };
        }

        private int GetTokenExpiryMinutes()
        {
            return _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 60);
        }
    }
}
