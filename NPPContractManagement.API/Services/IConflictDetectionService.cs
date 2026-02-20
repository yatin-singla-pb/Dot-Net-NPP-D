using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IConflictDetectionService
    {
        Task<ProposalConflictResultDto> DetectConflictsAsync(int proposalId);
    }
}
