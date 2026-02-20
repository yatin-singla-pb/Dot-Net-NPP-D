using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllContractsAsync();
        Task<ContractDto?> GetContractByIdAsync(int id);
        Task<ContractDto> CreateContractAsync(CreateContractDto createContractDto, string createdBy);
        Task<ContractDto> UpdateContractAsync(int id, UpdateContractDto updateContractDto, string modifiedBy);
        Task<bool> DeleteContractAsync(int id);

        Task<IEnumerable<ContractDto>> GetContractsByManufacturerIdAsync(int manufacturerId);
        Task<IEnumerable<ContractDto>> GetContractsByStatusAsync(int status);
        Task<IEnumerable<ContractDto>> GetContractsByDistributorIdAsync(int distributorId);
        Task<IEnumerable<ContractDto>> GetContractsByOpCoIdAsync(int opCoId);
        Task<IEnumerable<ContractDto>> GetContractsByIndustryIdAsync(int industryId);
        Task<IEnumerable<ContractDto>> GetSuspendedContractsAsync();
        Task<IEnumerable<ContractDto>> GetContractsForPerformanceAsync();
        Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(DateTime beforeDate);
        Task<IEnumerable<ContractDto>> GetExpiringContractsWithoutProposalsAsync(int daysThreshold);
        Task<bool> SuspendContractAsync(int id, string modifiedBy);
        Task<bool> UnsuspendContractAsync(int id, string modifiedBy);
        Task<bool> SendToPerformanceAsync(int id, string modifiedBy);
        Task<bool> RemoveFromPerformanceAsync(int id, string modifiedBy);
        Task<ContractDto> CreateNewVersionAsync(int contractId, string changeReason, string createdBy);
        Task<IEnumerable<ContractVersionDto>> GetVersionsAsync(int contractId);
        Task<ContractVersionDto?> GetVersionAsync(int contractId, int versionId);
        Task<ContractVersionDto> CreateVersionAsync(int contractId, CreateContractVersionRequest request, string createdBy);
        Task<ContractVersionDto> UpdateVersionAsync(int contractId, int versionId, UpdateContractVersionRequest request, string modifiedBy);
        Task<(IEnumerable<ContractDto> Contracts, int TotalCount)> SearchContractsAsync(string searchTerm, int? manufacturerId = null, int? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, IEnumerable<int>? allowedManufacturerIds = null);

        Task<ContractVersionDto> CloneVersionByNumberAsync(int contractId, int sourceVersionNumber, string createdBy);
        Task<object> CompareVersionsAsync(int contractId, int versionANumber, int versionBNumber);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<int> SendExpiryNotificationsAsync(int daysThreshold);
    }
}
