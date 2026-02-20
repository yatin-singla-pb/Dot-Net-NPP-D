using NPPContractManagement.API.Domain.Proposals.Entities;
using NPPContractManagement.API.DTOs.Proposals;

namespace NPPContractManagement.API.Services
{
    public interface IProposalService
    {
        Task<(IEnumerable<ProposalDto> Items, int TotalCount)> QueryProposalsAsync(
            string? search,
            int page,
            int pageSize,
            IEnumerable<int>? manufacturerIds = null,
            int? proposalStatusId = null,
            int? proposalTypeId = null,
            int? manufacturerId = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            DateTime? endDateFrom = null,
            DateTime? endDateTo = null,
            DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null,
            int? idFrom = null,
            int? idTo = null,
            string? sortBy = null,
            string? sortDirection = null);
        Task<ProposalDto?> GetProposalByIdAsync(int id);
        Task<ProposalDto> CreateProposalAsync(ProposalCreateDto dto, string createdBy);
        Task<ProposalDto> UpdateProposalAsync(int id, ProposalUpdateDto dto, string modifiedBy);
        Task<ProposalDto> CloneProposalAsync(int id, string createdBy);
        Task<bool> SubmitProposalAsync(int id, string userId);
        Task<bool> AcceptProductsAsync(int id, string userId);
        Task<bool> RejectProposalAsync(int id, string userId, string? rejectReason);
        Task<int> BatchCreateAsync(IEnumerable<ProposalCreateDto> items, string createdBy);
    }
}

