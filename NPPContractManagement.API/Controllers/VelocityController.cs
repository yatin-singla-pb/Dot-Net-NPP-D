using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Services;
using NPPContractManagement.API.Extensions;
using System.Text;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VelocityController : ControllerBase
    {
        private readonly IVelocityService _velocityService;
        private readonly IVelocityUsageReportService _usageReportService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VelocityController> _logger;
        private readonly IVelocityRepository _velocityRepository;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public VelocityController(
            IVelocityService velocityService,
            IVelocityUsageReportService usageReportService,
            ApplicationDbContext context,
            ILogger<VelocityController> logger,
            IVelocityRepository velocityRepository)
        {
            _velocityService = velocityService;
            _usageReportService = usageReportService;
            _context = context;
            _logger = logger;
            _velocityRepository = velocityRepository;
        }

        /// <summary>
        /// Ingest velocity data from CSV or Excel file upload
        /// </summary>
        [HttpPost("ingest")]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<ActionResult<VelocityIngestResponse>> IngestFile(IFormFile file, [FromForm] int distributorId)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { message = $"File size exceeds maximum of {MaxFileSize / 1024 / 1024} MB" });
                }

                // Validate distributor
                if (distributorId <= 0)
                {
                    return BadRequest(new { message = "Distributor ID is required" });
                }

                var distributor = await _context.Distributors.FindAsync(distributorId);
                if (distributor == null)
                {
                    return BadRequest(new { message = $"Distributor with ID {distributorId} not found" });
                }

                var fileName = Path.GetFileName(file.FileName); // Sanitize filename
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Only CSV (.csv) and Excel (.xlsx, .xls) files are allowed" });
                }

                var createdBy = User.Identity?.Name ?? "unknown";

                using var stream = file.OpenReadStream();
                var response = await _velocityService.IngestFromFileAsync(stream, fileName, createdBy, distributorId);

                _logger.LogInformation("File {FileName} ingested by {User} for distributor {DistributorId}, JobId: {JobId}",
                    fileName, createdBy, distributorId, response.JobId);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid CSV file uploaded");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ingesting file");
                return StatusCode(500, new { message = "An error occurred while processing the file", error = ex.Message });
            }
        }

        /// <summary>
        /// Get job details by job ID
        /// </summary>
        [HttpGet("jobs/{jobId}")]
        public async Task<ActionResult<VelocityJobDetailDto>> GetJob(string jobId)
        {
            try
            {
                var job = await _velocityService.GetJobDetailsAsync(jobId);

                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobId} not found" });
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", jobId);
                return StatusCode(500, new { message = "An error occurred while retrieving the job", error = ex.Message });
            }
        }

        /// <summary>
        /// Get paginated list of jobs
        /// </summary>
        [HttpGet("jobs")]
        public async Task<ActionResult<object>> GetJobs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null)
        {
            try
            {
                VelocityJobStatus? statusEnum = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<VelocityJobStatus>(status, true, out var parsedStatus))
                {
                    statusEnum = parsedStatus;
                }

                var (jobs, totalCount) = await _velocityService.GetJobsAsync(page, pageSize, statusEnum);

                return Ok(new
                {
                    data = jobs,
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return StatusCode(500, new { message = "An error occurred while retrieving jobs", error = ex.Message });
            }
        }

        /// <summary>
        /// Download sample CSV template
        /// </summary>
        [HttpGet("template")]
        public IActionResult GetTemplate()
        {
            try
            {
                var csv = _velocityService.GetSampleCsvTemplate();
                var bytes = Encoding.UTF8.GetBytes(csv);

                return File(bytes, "text/csv", "velocity_template.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template");
                return StatusCode(500, new { message = "An error occurred while generating the template", error = ex.Message });
            }
        }

        /// <summary>
        /// Retry or reprocess a failed/stuck job
        /// </summary>
        [HttpPost("jobs/{jobId}/retry")]
        public async Task<ActionResult> RetryJob(string jobId)
        {
            try
            {
                var job = await _context.VelocityJobs.FirstOrDefaultAsync(j => j.JobId == jobId);
                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobId} not found" });
                }

                // Reset job to failed status if stuck in processing
                if (job.Status == "processing")
                {
                    job.Status = "failed";
                    job.ErrorMessage = "Job was stuck in processing state and has been marked as failed";
                    job.CompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Job status updated", status = job.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying job {JobId}", jobId);
                return StatusCode(500, new { message = "An error occurred while retrying the job", error = ex.Message });
            }
        }

        /// <summary>
        /// Restart a failed or stuck job
        /// </summary>
        [HttpPost("jobs/{jobId}/restart")]
        public async Task<ActionResult<VelocityIngestResponse>> RestartJob(string jobId)
        {
            try
            {
                var result = await _velocityService.RestartJobAsync(jobId);

                if (result.Status == "error")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting job {JobId}", jobId);
                return StatusCode(500, new VelocityIngestResponse
                {
                    JobId = jobId,
                    Status = "error",
                    Message = $"An error occurred while restarting the job: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get velocity exceptions (failed job rows)
        /// </summary>
        [HttpPost("exceptions")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<VelocityExceptionsResponse>> GetExceptions([FromBody] VelocityExceptionsRequest request)
        {
            try
            {
                (List<VelocityJobRow> rows, int totalCount) = await _velocityRepository.GetFailedJobRowsAsync(
                    request.JobId,
                    request.StartDate,
                    request.EndDate,
                    request.Keyword,
                    request.Page,
                    request.PageSize);

                var dtos = rows.Select(r => new VelocityExceptionDto
                {
                    Id = r.Id,
                    JobId = r.JobId,
                    JobIdStr = r.JobId.ToString(),
                    RowIndex = r.RowIndex,
                    Status = r.Status,
                    ErrorMessage = r.ErrorMessage,
                    RawData = r.RawValues,
                    ProcessedAt = r.CreatedAt,
                    FileName = r.VelocityJob?.IngestedFile?.OriginalFilename,
                    CreatedBy = r.VelocityJob?.CreatedBy,
                    ActionStatus = r.ActionStatus,
                    ActionNotes = r.ActionNotes,
                    ActionTakenBy = r.ActionTakenBy,
                    ActionTakenAt = r.ActionTakenAt
                }).ToList();

                return Ok(new VelocityExceptionsResponse
                {
                    Data = dtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting velocity exceptions");
                return StatusCode(500, new { message = "An error occurred while retrieving exceptions", error = ex.Message });
            }
        }

        /// <summary>
        /// Perform an action on a velocity exception (dismiss, new contract, amendment)
        /// </summary>
        [HttpPost("exceptions/{id}/action")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> PerformExceptionAction(int id, [FromBody] VelocityExceptionActionRequest request)
        {
            try
            {
                var validActions = new HashSet<string> { "Dismissed", "NewContract", "Amendment" };
                if (!validActions.Contains(request.Action))
                    return BadRequest(new { message = "Invalid action. Must be: Dismissed, NewContract, or Amendment" });

                var row = await _context.Set<VelocityJobRow>().FindAsync(id);
                if (row == null)
                    return NotFound(new { message = "Exception not found" });

                var userName = HttpContext.GetAuditPrincipal();
                row.ActionStatus = request.Action;
                row.ActionNotes = request.Notes;
                row.ActionTakenBy = userName;
                row.ActionTakenAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Exception marked as '{request.Action}'" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing action on velocity exception {Id}", id);
                return StatusCode(500, new { message = "An error occurred while performing the action" });
            }
        }

        /// <summary>
        /// Generate velocity usage report
        /// </summary>
        [HttpPost("usage-report")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<VelocityUsageReportResponse>> GetUsageReport([FromBody] VelocityUsageReportRequest request)
        {
            try
            {
                var response = await _usageReportService.GenerateReportAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating velocity usage report");
                return StatusCode(500, new { message = "An error occurred while generating the report", error = ex.Message });
            }
        }

        /// <summary>
        /// Get detail records for a specific aggregate group
        /// </summary>
        [HttpPost("usage-report/details")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<List<VelocityUsageDetailDto>>> GetUsageDetails([FromBody] GetUsageDetailsRequest request)
        {
            try
            {
                var details = await _usageReportService.GetDetailRecordsAsync(request.GroupKey, request.ReportRequest);
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage details");
                return StatusCode(500, new { message = "An error occurred while retrieving details", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if contracts exist for selected products
        /// </summary>
        [HttpPost("usage-report/check-contracts")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<Dictionary<string, List<int>>>> CheckContracts([FromBody] List<string> groupKeys)
        {
            try
            {
                var result = await _usageReportService.CheckExistingContractsAsync(groupKeys);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking contracts");
                return StatusCode(500, new { message = "An error occurred while checking contracts", error = ex.Message });
            }
        }

        /// <summary>
        /// Create proposals from velocity data
        /// </summary>
        [HttpPost("usage-report/create-proposals")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<List<int>>> CreateProposals([FromBody] CreateProposalFromVelocityRequest request)
        {
            try
            {
                var createdBy = User.Identity?.Name ?? "System";
                var proposalIds = await _usageReportService.CreateProposalsFromVelocityAsync(request, createdBy);
                return Ok(proposalIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating proposals from velocity data");
                return StatusCode(500, new { message = "An error occurred while creating proposals", error = ex.Message });
            }
        }

        public class GetUsageDetailsRequest
        {
            public string GroupKey { get; set; } = string.Empty;
            public VelocityUsageReportRequest ReportRequest { get; set; } = new();
        }
    }
}

