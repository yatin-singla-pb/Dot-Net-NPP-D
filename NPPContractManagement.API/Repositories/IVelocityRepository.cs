using NPPContractManagement.API.Models;
using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Repositories
{
    public interface IVelocityRepository
    {
        // IngestedFile operations
        Task<IngestedFile> CreateIngestedFileAsync(IngestedFile file);
        Task<IngestedFile?> GetIngestedFileByIdAsync(int id);

        // Job operations
        Task<VelocityJob> CreateJobAsync(VelocityJob job);
        Task<VelocityJob?> GetJobByJobIdAsync(string jobId);
        Task<VelocityJob?> GetJobByIdAsync(int id);
        Task UpdateJobAsync(VelocityJob job);
        Task<List<VelocityJob>> GetJobsAsync(int page, int pageSize, VelocityJobStatus? status = null);
        Task<int> GetJobsCountAsync(VelocityJobStatus? status = null);
        Task<List<VelocityJob>> GetJobsByStatusAsync(string status);

        // Job data for resume capability
        Task StoreJobDataAsync(int jobId, List<VelocityValidationResult> validationResults, string createdBy);
        Task<VelocityJobData?> GetJobDataAsync(int jobId);
        Task DeleteJobDataAsync(int jobId);

        // Job row operations
        Task<VelocityJobRow> CreateJobRowAsync(VelocityJobRow row);
        Task CreateJobRowsBatchAsync(List<VelocityJobRow> rows);
        Task<List<VelocityJobRow>> GetJobRowsAsync(int jobId);
        Task UpdateJobRowAsync(VelocityJobRow row);

        // Shipment operations
        Task<VelocityShipment> CreateShipmentAsync(VelocityShipment shipment);
        Task CreateShipmentsBatchAsync(List<VelocityShipment> shipments);
        Task<List<VelocityShipment>> GetShipmentsAsync(int page, int pageSize, int? distributorId = null, int? jobId = null);
        Task<int> GetShipmentsCountAsync(int? distributorId = null, int? jobId = null);

        // sFTP config operations
        Task<SftpProbeConfig> CreateSftpConfigAsync(SftpProbeConfig config);
        Task<SftpProbeConfig?> GetSftpConfigByIdAsync(int id);
        Task<List<SftpProbeConfig>> GetSftpConfigsAsync();
        Task UpdateSftpConfigAsync(SftpProbeConfig config);
        Task DeleteSftpConfigAsync(int id);

        // Velocity Exceptions operations
        Task<(List<VelocityJobRow> Rows, int TotalCount)> GetFailedJobRowsAsync(
            int? jobId,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            int page,
            int pageSize);

        // Velocity Usage Report operations
        Task<(List<VelocityShipment> Shipments, int TotalCount)> GetVelocityUsageDataAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            List<int>? manufacturerIds,
            List<int>? opCoIds,
            List<int>? industryIds,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDirection);
    }
}

