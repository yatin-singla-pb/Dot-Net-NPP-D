using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IMemberAccountService
    {
        Task<IEnumerable<MemberAccountDto>> GetAllMemberAccountsAsync();
        Task<MemberAccountDto?> GetMemberAccountByIdAsync(int id);
        Task<MemberAccountDto> CreateMemberAccountAsync(CreateMemberAccountDto createMemberAccountDto, string createdBy);
        Task<MemberAccountDto> UpdateMemberAccountAsync(int id, UpdateMemberAccountDto updateMemberAccountDto, string modifiedBy);
        Task<bool> DeleteMemberAccountAsync(int id);
        Task<bool> ActivateMemberAccountAsync(int id, string modifiedBy);
        Task<bool> DeactivateMemberAccountAsync(int id, string modifiedBy);
        Task<MemberAccountDto?> GetMemberAccountByMemberNumberAsync(string memberNumber);
        Task<IEnumerable<MemberAccountDto>> GetMemberAccountsByIndustryIdAsync(int industryId);
        Task<IEnumerable<MemberAccountDto>> GetMemberAccountsByStatusAsync(int status);
        Task<(IEnumerable<MemberAccountDto> MemberAccounts, int TotalCount)> SearchMemberAccountsAsync(string searchTerm, int? industryId = null, int? status = null, string? w9 = null, string? state = null, int page = 1, int pageSize = 10);
    }
}
