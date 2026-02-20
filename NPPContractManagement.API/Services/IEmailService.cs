namespace NPPContractManagement.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task<bool> SendTemporaryPasswordEmailAsync(string toEmail, string userName, string temporaryPassword);
        Task<bool> SendUserInvitationEmailAsync(string toEmail, string userName, string invitationLink);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendVelocityJobCompletionEmailAsync(string toEmail, string userName, string jobId, int totalRows, int successRows, int failedRows, string status, string detailsUrl);

        /// <summary>
        /// Notify manufacturer-classed users that NPP has created a new proposal request.
        /// </summary>
        /// <returns>true if email was queued/sent successfully; false otherwise.</returns>
        Task<bool> SendProposalRequestedEmailAsync(
            string toEmail,
            string userName,
            string proposalTitle,
            int proposalId,
            DateTime? startDate,
            DateTime? endDate);

        Task<bool> SendRegistrationInvitationEmailAsync(string toEmail, string userName, string registrationLink);
        Task<bool> SendVerificationCodeEmailAsync(string toEmail, string userName, string code);

        Task<bool> SendProposalSubmittedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId);
        Task<bool> SendProposalAcceptedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId);
        Task<bool> SendProposalRejectedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId, string? rejectReason);
        Task<bool> SendContractExpiryNotificationEmailAsync(string toEmail, string userName, string contractName, int contractId, DateTime endDate, int daysUntilExpiry);
    }
}
