using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Repositories;
using BCrypt.Net;
using System.Security.Cryptography;

namespace NPPContractManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IUserManufacturerRepository _userManufacturerRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IUserManufacturerRepository userManufacturerRepository,
            IEmailService emailService,
            ILogger<UserService> logger,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userManufacturerRepository = userManufacturerRepository;
            _emailService = emailService;
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var userWithRoles = await _userRepository.GetUserWithRolesAsync(user.Id);
                    if (userWithRoles != null)
                    {
                        userDtos.Add(MapUserToDto(userWithRoles));
                    }
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchUsersAsync(string searchTerm, bool? isActive, int pageNumber, int pageSize, string? sortBy, string sortDirection, Models.AccountStatus? accountStatus = null)
        {
            try
            {
                var users = await _userRepository.SearchAsync(searchTerm, isActive, pageNumber, pageSize, sortBy, sortDirection, accountStatus);
                var total = await _userRepository.GetCountAsync(searchTerm, isActive, accountStatus);
                var userDtos = users.Select(MapUserToDto);
                return (userDtos, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                throw;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(id);
                return user != null ? MapUserToDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id {Id}", id);
                throw;
            }
        }

        public async Task<UserDto?> GetUserByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithRolesByUserIdAsync(userId);
                return user != null ? MapUserToDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by userId {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
        {
            try
            {
                bool isHeadless = string.IsNullOrWhiteSpace(createUserDto.UserId);

                // Check if UserId already exists (only for non-headless)
                if (!isHeadless && await _userRepository.IsUserIdTakenAsync(createUserDto.UserId!))
                {
                    throw new InvalidOperationException($"User ID '{createUserDto.UserId}' is already taken");
                }

                if (await _userRepository.IsEmailTakenAsync(createUserDto.Email))
                {
                    throw new InvalidOperationException($"Email '{createUserDto.Email}' is already taken");
                }

                // Validate roles exist
                var roles = new List<Role>();
                foreach (var roleId in createUserDto.RoleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null)
                    {
                        throw new InvalidOperationException($"Role with ID {roleId} not found");
                    }
                    roles.Add(role);
                }

                // Generate a secure temporary password (only for non-headless)
                var temporaryPassword = isHeadless ? null : GenerateSecureTemporaryPassword();

                // Create user
                var user = new User
                {
                    UserId = isHeadless ? null : createUserDto.UserId,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    Email = createUserDto.Email,
                    PhoneNumber = createUserDto.PhoneNumber,
                    Company = createUserDto.Company,
                    JobTitle = createUserDto.JobTitle,
                    Address = createUserDto.Address,
                    City = createUserDto.City,
                    State = createUserDto.State,
                    PostCode = createUserDto.PostCode,
                    Notes = createUserDto.Notes,
                    IndustryId = createUserDto.IndustryId,
                    GroupEmail = createUserDto.GroupEmail,
                    AccountStatus = isHeadless ? AccountStatus.Headless : (Models.AccountStatus)createUserDto.AccountStatus,
                    Status = createUserDto.Status,
                    Class = createUserDto.Class,
                    FailedAuthAttempts = createUserDto.FailedAuthAttempts,
                    PasswordHash = isHeadless ? null : BCrypt.Net.BCrypt.HashPassword(temporaryPassword!),
                    IsHeadless = isHeadless,
                    IsActive = true,
                    EmailConfirmed = false,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdUser = await _userRepository.AddAsync(user);

                // Assign roles
                foreach (var role in roles)
                {
                    var userRole = new UserRole
                    {
                        UserId = createdUser.Id,
                        RoleId = role.Id,
                        AssignedDate = DateTime.UtcNow,
                        AssignedBy = createdBy
                    };
                    await _userRoleRepository.AddAsync(userRole);
                }

                // Sync manufacturers in same flow
                var isManufacturerRole = roles.Any(r => string.Equals(r.Name, "Manufacturer", StringComparison.OrdinalIgnoreCase));
                var targetManufacturerIds = isManufacturerRole ? (createUserDto.ManufacturerIds ?? new List<int>()) : new List<int>();
                await _userManufacturerRepository.SyncUserManufacturersAsync(createdUser.Id, targetManufacturerIds, createdBy);

                // Send email only for non-headless accounts
                // Headless accounts are used as simple contact objects; invitation can be sent manually later
                if (!isHeadless)
                {
                    try
                    {
                        var emailSent = await _emailService.SendTemporaryPasswordEmailAsync(
                            createdUser.Email,
                            $"{createdUser.FirstName} {createdUser.LastName}",
                            temporaryPassword!);

                        if (!emailSent)
                        {
                            _logger.LogError("Failed to send temporary password email to user {UserId}", createdUser.UserId);
                        }
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send email to user {Email}", createdUser.Email);
                    }
                }

                _logger.LogInformation("User created successfully (headless={IsHeadless}) by {CreatedBy}", isHeadless, createdBy);

                // Return user with roles
                var userWithRoles = await _userRepository.GetUserWithRolesAsync(createdUser.Id);
                return MapUserToDto(userWithRoles!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", createUserDto.Email);
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto, string modifiedBy)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                // Check if email is taken by another user
                if (await _userRepository.IsEmailTakenAsync(updateUserDto.Email, id))
                {
                    throw new InvalidOperationException($"Email '{updateUserDto.Email}' is already taken by another user");
                }

                // Validate roles exist
                var roles = new List<Role>();
                foreach (var roleId in updateUserDto.RoleIds)
                {
                    var role = await _roleRepository.GetByIdAsync(roleId);
                    if (role == null)
                    {
                        throw new InvalidOperationException($"Role with ID {roleId} not found");
                    }
                    roles.Add(role);
                }

                // Update user properties
                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                user.Email = updateUserDto.Email;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.Company = updateUserDto.Company;
                user.JobTitle = updateUserDto.JobTitle;
                user.Address = updateUserDto.Address;
                user.City = updateUserDto.City;
                user.State = updateUserDto.State;
                user.PostCode = updateUserDto.PostCode;
                user.Notes = updateUserDto.Notes;
                user.IndustryId = updateUserDto.IndustryId;
                user.GroupEmail = updateUserDto.GroupEmail;
                user.AccountStatus = (Models.AccountStatus)updateUserDto.AccountStatus;
                user.Status = updateUserDto.Status;
                user.Class = updateUserDto.Class;
                user.IsActive = updateUserDto.IsActive;
                user.FailedAuthAttempts = updateUserDto.FailedAuthAttempts;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = modifiedBy;

                await _userRepository.UpdateAsync(user);

                // Update user roles
                var existingUserRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == id);
                foreach (var existingUserRole in existingUserRoles)
                {
                    await _userRoleRepository.DeleteAsync(existingUserRole);
                }

                foreach (var role in roles)
                {
                    var userRole = new UserRole
                    {
                        UserId = id,
                        RoleId = role.Id,
                        AssignedDate = DateTime.UtcNow,
                        AssignedBy = modifiedBy
                    };
                    await _userRoleRepository.AddAsync(userRole);
                }

                // Sync manufacturers in same flow
                var isManufacturerRole = roles.Any(r => string.Equals(r.Name, "Manufacturer", StringComparison.OrdinalIgnoreCase));
                var targetManufacturerIds = isManufacturerRole ? (updateUserDto.ManufacturerIds ?? new List<int>()) : new List<int>();
                await _userManufacturerRepository.SyncUserManufacturersAsync(id, targetManufacturerIds, modifiedBy);

                _logger.LogInformation("User {UserId} updated successfully by {ModifiedBy}", user.UserId, modifiedBy);

                // Return updated user with roles
                var userWithRoles = await _userRepository.GetUserWithRolesAsync(id);
                return MapUserToDto(userWithRoles!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                // Soft delete: mark as inactive instead of removing the record
                if (!user.IsActive)
                {
                    return true; // already inactive
                }

                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} soft-deleted (IsActive=false)", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting user {Id}", id);
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(int id, string modifiedBy)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = true;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = modifiedBy;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} activated by {ModifiedBy}", user.UserId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(int id, string modifiedBy)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = modifiedBy;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} deactivated by {ModifiedBy}", user.UserId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {Id}", id);
                throw;
            }
        }

        public async Task<bool> SuspendUserAsync(int id, string modifiedBy)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.AccountStatus = AccountStatus.Suspended;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = modifiedBy;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} suspended by {ModifiedBy}", user.UserId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending user {Id}", id);
                throw;
            }
        }

        public async Task<bool> UnsuspendUserAsync(int id, string modifiedBy)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.AccountStatus = AccountStatus.Active;
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = modifiedBy;

                await _userRepository.UpdateAsync(user);
                _logger.LogInformation("User {UserId} unsuspended by {ModifiedBy}", user.UserId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsuspending user {Id}", id);
                throw;
            }
        }

        public async Task<string> SendUserInvitationAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} not found");
                }

                var invitationToken = GenerateInvitationToken(user.Email);
                var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4200";
                var invitationLink = $"{frontendUrl}/set-password?token={invitationToken}&email={user.Email}";

                await _emailService.SendUserInvitationEmailAsync(
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    invitationLink);

                _logger.LogInformation("Invitation email sent to user {UserId}", user.UserId);
                return invitationToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invitation to user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto)
        {
            try
            {
                // Validate token (simplified implementation)
                if (!ValidateInvitationToken(setPasswordDto.Token))
                {
                    return false;
                }

                var email = ExtractEmailFromToken(setPasswordDto.Token);
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(setPasswordDto.Password);
                user.EmailConfirmed = true;
                user.ModifiedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");

                _logger.LogInformation("Password set successfully for user {UserId}", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting password");
                throw;
            }
        }


        private string GenerateSecureTemporaryPassword(int length = 12)
        {
            if (length < 8) length = 8;

            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ"; // exclude I, O
            const string lower = "abcdefghijkmnopqrstuvwxyz"; // exclude l
            const string digits = "23456789"; // exclude 0,1
            const string special = "!@$?_-";
            var all = upper + lower + digits + special;

            var chars = new char[length];
            // ensure at least one of each category
            chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
            chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
            chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
            chars[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

            for (int i = 4; i < length; i++)
            {
                chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
            }

            // Fisher–Yates shuffle
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
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
                Company = user.Company,
                JobTitle = user.JobTitle,
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostCode = user.PostCode,
                Notes = user.Notes,
                IndustryId = user.IndustryId,
                GroupEmail = user.GroupEmail,
                AccountStatus = (int)user.AccountStatus,
                Status = user.Status,
                Class = user.Class,
                IsActive = user.IsActive,
                IsHeadless = user.IsHeadless,
                EmailConfirmed = user.EmailConfirmed,
                FailedAuthAttempts = user.FailedAuthAttempts,
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

        private string GenerateInvitationToken(string email)
        {
            var tokenData = $"{email}:{DateTime.UtcNow.AddDays(1):yyyy-MM-dd HH:mm:ss}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        private bool ValidateInvitationToken(string token)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(token);
                var tokenData = System.Text.Encoding.UTF8.GetString(tokenBytes);

                // Token format is "email:yyyy-MM-dd HH:mm:ss" — use IndexOf to split
                // because the datetime portion contains colons
                var separatorIndex = tokenData.IndexOf(':');
                if (separatorIndex < 0)
                {
                    return false;
                }

                var expiryString = tokenData.Substring(separatorIndex + 1);
                if (DateTime.TryParse(expiryString, out var expiry))
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

        private string ExtractEmailFromToken(string token)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(token);
                var tokenData = System.Text.Encoding.UTF8.GetString(tokenBytes);
                var parts = tokenData.Split(':');
                return parts[0];
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            // Check if email is taken by another user
            if (await _userRepository.IsEmailTakenAsync(dto.Email, userId))
            {
                throw new InvalidOperationException($"Email '{dto.Email}' is already taken by another user");
            }

            // Update only personal profile fields
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Company = dto.Company;
            user.JobTitle = dto.JobTitle;
            user.Address = dto.Address;
            user.City = dto.City;
            user.State = dto.State;
            user.PostCode = dto.PostCode;
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifiedBy = user.UserId ?? $"User:{userId}";

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Profile updated for user {UserId}", userId);

            var userWithRoles = await _userRepository.GetUserWithRolesAsync(userId);
            return MapUserToDto(userWithRoles!);
        }
    }
}
