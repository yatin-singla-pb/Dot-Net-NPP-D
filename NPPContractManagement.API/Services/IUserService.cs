using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<(IEnumerable<UserDto> Users, int TotalCount)> SearchUsersAsync(string searchTerm, bool? isActive, int pageNumber, int pageSize, string? sortBy, string sortDirection, Models.AccountStatus? accountStatus = null);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUserIdAsync(string userId);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto, string modifiedBy);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ActivateUserAsync(int id, string modifiedBy);
        Task<bool> DeactivateUserAsync(int id, string modifiedBy);
        Task<bool> SuspendUserAsync(int id, string modifiedBy);
        Task<bool> UnsuspendUserAsync(int id, string modifiedBy);
        Task<string> SendUserInvitationAsync(int userId);
        Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto);
        Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
    }
}
