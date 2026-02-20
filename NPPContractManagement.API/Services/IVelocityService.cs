using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Services
{
    public interface IVelocityService
    {
        /// <summary>
        /// Ingest velocity data from a CSV or Excel file stream
        /// </summary>
        Task<VelocityIngestResponse> IngestFromFileAsync(Stream fileStream, string fileName, string createdBy, int distributorId);

        /// <summary>
        /// Ingest velocity data from an sFTP location
        /// </summary>
        Task<VelocityIngestResponse> IngestFromSftpAsync(string sftpFileUrl, string createdBy, int distributorId, Dictionary<string, string>? jobMeta = null);

        /// <summary>
        /// Get job details by job ID
        /// </summary>
        Task<VelocityJobDetailDto?> GetJobDetailsAsync(string jobId);

        /// <summary>
        /// Get paginated list of jobs
        /// </summary>
        Task<(List<VelocityJobDto> Jobs, int TotalCount)> GetJobsAsync(int page, int pageSize, VelocityJobStatus? status = null);

        /// <summary>
        /// Process a queued job (called by background worker)
        /// </summary>
        Task ProcessJobAsync(int jobId);

        /// <summary>
        /// Resume an incomplete job after restart
        /// </summary>
        Task ResumeJobAsync(int jobId, string createdBy);

        /// <summary>
        /// Restart a failed or stuck job
        /// </summary>
        Task<VelocityIngestResponse> RestartJobAsync(string jobId);

        /// <summary>
        /// Get sample CSV template content
        /// </summary>
        string GetSampleCsvTemplate();
    }
}

