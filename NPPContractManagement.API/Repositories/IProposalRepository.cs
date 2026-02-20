using NPPContractManagement.API.Domain.Proposals.Entities;

namespace NPPContractManagement.API.Repositories
{
    public interface IProposalRepository
    {
        Task<(IEnumerable<Proposal> Items, int TotalCount)> QueryAsync(
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
        Task<Proposal?> GetByIdAsync(int id);
        Task<Proposal> AddAsync(Proposal entity);
        Task UpdateAsync(Proposal entity);
        Task DeleteAsync(int id);
    }
}

