using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IBulkRenewalService
    {
        /// <summary>
        /// Create multiple proposal requests from selected contracts
        /// </summary>
        Task<BulkRenewalResponse> CreateBulkRenewalProposalsAsync(BulkRenewalRequest request);

        /// <summary>
        /// Validate contracts can be renewed
        /// </summary>
        Task<Dictionary<int, string>> ValidateContractsForRenewalAsync(List<int> contractIds);
    }
}

