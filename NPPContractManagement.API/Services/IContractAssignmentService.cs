using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Services
{
    public interface IContractAssignmentService
    {
        // Distributors
        Task<List<ContractDistributor>> GetDistributorsAsync(int contractId);
        Task<ContractDistributor> AddDistributorAsync(int contractId, int distributorId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate);
        Task RemoveDistributorAsync(int id);

        // Manufacturers
        Task<List<ContractManufacturer>> GetManufacturersAsync(int contractId);
        Task<ContractManufacturer> AddManufacturerAsync(int contractId, int manufacturerId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate);
        Task RemoveManufacturerAsync(int id);

        // OpCos
        Task<List<ContractOpCo>> GetOpCosAsync(int contractId);
        Task<ContractOpCo> AddOpCoAsync(int contractId, int opCoId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate);
        Task RemoveOpCoAsync(int id);

        // Industries
        Task<List<ContractIndustry>> GetIndustriesAsync(int contractId);
        Task<ContractIndustry> AddIndustryAsync(int contractId, int industryId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate);
        Task RemoveIndustryAsync(int id);
    }
}

