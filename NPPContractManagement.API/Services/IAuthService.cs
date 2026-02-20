using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ValidatePasswordResetTokenAsync(string token, string email);
        Task LogoutAsync(int userId);
        string GenerateJwtToken(UserDto user);
        string GenerateRefreshToken();
    }
}
