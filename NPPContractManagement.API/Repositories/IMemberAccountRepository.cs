using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IMemberAccountRepository : IRepository<MemberAccount>
    {
        Task<MemberAccount?> GetByMemberNumberAsync(string memberNumber);
        Task<bool> ExistsByMemberNumberAsync(string memberNumber, int? excludeId = null);
        Task<IEnumerable<MemberAccount>> GetByIndustryIdAsync(int industryId);
        Task<IEnumerable<MemberAccount>> GetByStatusAsync(MemberAccountStatus status);
        Task<IEnumerable<MemberAccount>> SearchAsync(string searchTerm, int? industryId = null, MemberAccountStatus? status = null, string? w9 = null, string? state = null, int page = 1, int pageSize = 10);
        Task<int> GetCountAsync(string? searchTerm = null, int? industryId = null, MemberAccountStatus? status = null, string? w9 = null, string? state = null);
    }
}
