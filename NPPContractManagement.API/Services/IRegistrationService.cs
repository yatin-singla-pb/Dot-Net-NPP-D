using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationInitiatedResponseDto> InitiateRegistrationAsync(string email);
        Task<VerifyCodeResponseDto?> VerifyCodeAsync(string email, string code);
        Task<CheckUserIdResponseDto> CheckUserIdAvailabilityAsync(string userId);
        Task<bool> CompleteRegistrationAsync(CompleteRegistrationDto dto);
        Task<bool> ResendInvitationAsync(int userId);
    }
}
