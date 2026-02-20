using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IVelocityUsageReportService
    {
        /// <summary>
        /// Generate aggregated velocity usage report
        /// </summary>
        Task<VelocityUsageReportResponse> GenerateReportAsync(VelocityUsageReportRequest request);

        /// <summary>
        /// Get individual velocity records for a specific group
        /// </summary>
        Task<List<VelocityUsageDetailDto>> GetDetailRecordsAsync(string groupKey, VelocityUsageReportRequest request);

        /// <summary>
        /// Check if contracts exist for selected products
        /// </summary>
        Task<Dictionary<string, List<int>>> CheckExistingContractsAsync(List<string> groupKeys);

        /// <summary>
        /// Create proposals from selected velocity data
        /// </summary>
        Task<List<int>> CreateProposalsFromVelocityAsync(CreateProposalFromVelocityRequest request, string createdBy);
    }
}

