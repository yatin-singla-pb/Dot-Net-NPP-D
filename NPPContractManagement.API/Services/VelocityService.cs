using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using System.Text.Json;

namespace NPPContractManagement.API.Services
{
    public class VelocityService : IVelocityService
    {
        private readonly IVelocityRepository _repository;
        private readonly IVelocityCsvParser _csvParser;
        private readonly IVelocityExcelParser _excelParser;
        private readonly ILogger<VelocityService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public VelocityService(
            IVelocityRepository repository,
            IVelocityCsvParser csvParser,
            IVelocityExcelParser excelParser,
            ILogger<VelocityService> logger,
            IEmailService emailService,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _repository = repository;
            _csvParser = csvParser;
            _excelParser = excelParser;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        public async Task<VelocityIngestResponse> IngestFromFileAsync(Stream fileStream, string fileName, string createdBy, int distributorId)
        {
            // Create IngestedFile record
            var ingestedFile = new IngestedFile
            {
                OriginalFilename = fileName,
                UploadedBy = createdBy,
                SourceType = "upload",
                Bytes = fileStream.CanSeek ? fileStream.Length : null,
                CreatedAt = DateTime.UtcNow
            };
            ingestedFile = await _repository.CreateIngestedFileAsync(ingestedFile);

            // Create job
            var job = new VelocityJob
            {
                FileId = ingestedFile.FileId,
                DistributorId = distributorId,
                Status = "queued",
                FileName = fileName,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            job = await _repository.CreateJobAsync(job);

            _logger.LogInformation("Created velocity job {JobId} for file {FileName} with distributor {DistributorId}",
                job.JobId, fileName, distributorId);

            // Determine file type and parse accordingly
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            List<VelocityValidationResult> validationResults;

            try
            {
                // Ensure stream is at the beginning
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                if (extension == ".xlsx" || extension == ".xls")
                {
                    _logger.LogInformation("Parsing Excel file {FileName}", fileName);
                    validationResults = await _excelParser.ParseAndValidateAsync(fileStream);
                }
                else if (extension == ".csv")
                {
                    _logger.LogInformation("Parsing CSV file {FileName}", fileName);
                    validationResults = await _csvParser.ParseAndValidateAsync(fileStream);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported file type: {extension}. Only .csv, .xlsx, and .xls files are supported.");
                }

                job.TotalRows = validationResults.Count;
                job.Status = "queued";
                await _repository.UpdateJobAsync(job);

                // Store validation results for resume capability
                await _repository.StoreJobDataAsync(job.Id, validationResults, createdBy);

                // Process in background using fire-and-forget
                _logger.LogInformation("Queuing job {JobId} with {RowCount} rows for background processing", job.JobId, validationResults.Count);

                // Start background processing in a new DI scope (the HTTP request scope will be disposed)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IVelocityService>();
                        _logger.LogInformation("Starting background processing for job {JobId} with {RowCount} rows", job.JobId, validationResults.Count);
                        await service.ResumeJobAsync(job.Id, createdBy);
                        _logger.LogInformation("Completed background processing for job {JobId}", job.JobId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background processing failed for job {JobId}", job.JobId);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file {FileName} for job {JobId}", fileName, job.JobId);
                job.Status = "failed";
                job.ErrorMessage = $"Error parsing file: {ex.Message}";
                job.CompletedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);
                throw;
            }

            return new VelocityIngestResponse
            {
                JobId = job.JobId,
                Status = "queued",
                DetailsUrl = $"/api/velocity/jobs/{job.JobId}",
                Message = $"Job queued for processing. {job.TotalRows} rows will be processed in the background. You can navigate away and check back later."
            };
        }

        public async Task<VelocityIngestResponse> IngestFromSftpAsync(string sftpFileUrl, string createdBy, int distributorId, Dictionary<string, string>? jobMeta = null)
        {
            // For now, return not implemented
            // In production, this would download from sFTP and call IngestFromFileAsync
            throw new NotImplementedException("sFTP ingestion will be implemented in phase 2");
        }

        public async Task<VelocityJobDetailDto?> GetJobDetailsAsync(string jobId)
        {
            var job = await _repository.GetJobByJobIdAsync(jobId);
            if (job == null) return null;

            var rows = await _repository.GetJobRowsAsync(job.Id);

            return new VelocityJobDetailDto
            {
                Id = job.Id,
                JobId = job.JobId,
                Status = job.Status.ToString(),
                FileName = job.FileName,
                SftpFileUrl = job.SftpFileUrl,
                TotalRows = job.TotalRows,
                ProcessedRows = job.ProcessedRows,
                SuccessRows = job.SuccessRows,
                FailedRows = job.FailedRows,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                CreatedBy = job.CreatedBy,
                ErrorMessage = job.ErrorMessage,
                DetailsUrl = $"/api/velocity/jobs/{job.JobId}",
                Rows = rows.Select(r => new VelocityJobRowDto
                {
                    Id = r.Id,
                    RowIndex = r.RowIndex,
                    Status = r.Status,
                    ErrorMessage = r.ErrorMessage,
                    RawData = r.RawData,
                    VelocityShipmentId = r.VelocityShipmentId,
                    ProcessedAt = r.ProcessedAt
                }).ToList()
            };
        }

        public async Task<(List<VelocityJobDto> Jobs, int TotalCount)> GetJobsAsync(int page, int pageSize, VelocityJobStatus? status = null)
        {
            var jobs = await _repository.GetJobsAsync(page, pageSize, status);
            var totalCount = await _repository.GetJobsCountAsync(status);

            var jobDtos = jobs.Select(j => new VelocityJobDto
            {
                Id = j.Id,
                JobId = j.JobId,
                Status = j.Status.ToString(),
                FileName = j.FileName,
                SftpFileUrl = j.SftpFileUrl,
                TotalRows = j.TotalRows,
                ProcessedRows = j.ProcessedRows,
                SuccessRows = j.SuccessRows,
                FailedRows = j.FailedRows,
                CreatedAt = j.CreatedAt,
                StartedAt = j.StartedAt,
                CompletedAt = j.CompletedAt,
                CreatedBy = j.CreatedBy,
                ErrorMessage = j.ErrorMessage,
                DetailsUrl = $"/api/velocity/jobs/{j.JobId}"
            }).ToList();

            return (jobDtos, totalCount);
        }

        public async Task ProcessJobAsync(int jobId)
        {
            var job = await _repository.GetJobByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogError("Job {JobId} not found", jobId);
                return;
            }

            try
            {
                job.Status = "processing";
                job.StartedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);

                _logger.LogInformation("Processing job {JobId}", job.JobId);

                // For file-based ingestion, we need to re-read the file
                // In production, you'd store the file temporarily or use a message queue
                // For now, we'll just mark as completed
                job.Status = "completed";
                job.CompletedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);

                _logger.LogInformation("Completed job {JobId}", job.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}", job.JobId);
                job.Status = "failed";
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);
            }
        }

        public string GetSampleCsvTemplate()
        {
            return @"OPCO,Customer #,Customer Name,Address One,Address Two,City,Zip Code,Invoice #,Invoice Date,Product #,Brand,Pack Size,Description,Corp Manuf #,GTIN,Manufacturer Name,Qty,Sales,Landed Cost,Allowances
001,CUST001,ABC Restaurant,123 Main St,Suite 100,New York,10001,INV-2024-001,2024-12-01,PROD001,Brand A,12x500ml,Product Description,CORP001,1234567890123,Manufacturer A,50,1250.00,1000.00,50.00
001,CUST002,XYZ Cafe,456 Oak Ave,,Los Angeles,90001,INV-2024-002,2024-12-01,PROD002,Brand B,24x250ml,Another Product,CORP002,9876543210987,Manufacturer B,100,2500.00,2000.00,100.00
002,CUST003,Restaurant Group,789 Pine Rd,Building 5,Chicago,60601,INV-2024-003,2024-12-02,PROD003,Brand C,6x1L,Third Product,CORP003,5555555555555,Manufacturer C,25,750.00,600.00,30.00";
        }

        private async Task ProcessCsvDataAsync(int jobId, string createdBy)
        {
            // Load job and validation results from database
            var job = await _repository.GetJobByIdAsync(jobId);
            if (job == null)
            {
                _logger.LogError("Job {JobId} not found", jobId);
                return;
            }

            try
            {

                var jobData = await _repository.GetJobDataAsync(jobId);
                if (jobData == null)
                {
                    _logger.LogError("Job data not found for job {JobId}", jobId);
                    job.Status = "failed";
                    job.ErrorMessage = "Job data not found. Cannot resume processing.";
                    job.CompletedAt = DateTime.UtcNow;
                    await _repository.UpdateJobAsync(job);
                    return;
                }

                var validationResults = JsonSerializer.Deserialize<List<VelocityValidationResult>>(jobData.ValidationResultsJson);
                if (validationResults == null || validationResults.Count == 0)
                {
                    _logger.LogError("Failed to deserialize validation results for job {JobId}", jobId);
                    job.Status = "failed";
                    job.ErrorMessage = "Failed to load job data. Cannot resume processing.";
                    job.CompletedAt = DateTime.UtcNow;
                    await _repository.UpdateJobAsync(job);
                    return;
                }

                _logger.LogInformation("ProcessCsvDataAsync started for job {JobId} with {RowCount} rows", job.JobId, validationResults.Count);
                job.Status = "processing";
                job.StartedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);

                int successCount = 0;
                int failedCount = 0;
                int logInterval = Math.Max(1, validationResults.Count / 10); // Log every 10%

                // Batch processing configuration
                const int batchSize = 1000;
                var shipmentBatch = new List<VelocityShipment>();
                var jobRowBatch = new List<VelocityJobRow>();

            for (int i = 0; i < validationResults.Count; i++)
            {
                var result = validationResults[i];
                var rowIndex = i + 2; // +2 because row 1 is header, index starts at 0

                // Log progress every 10%
                if (i % logInterval == 0)
                {
                    _logger.LogInformation("Processing job {JobId}: {Progress}% complete ({Current}/{Total})",
                        job.JobId, (i * 100 / validationResults.Count), i, validationResults.Count);
                }

                var jobRow = new VelocityJobRow
                {
                    VelocityJobId = job.Id,
                    RowIndex = rowIndex,
                    FileId = job.FileId ?? 0,
                    RawData = JsonSerializer.Serialize(result.Row),
                    ProcessedAt = DateTime.UtcNow
                };

                if (result.IsValid && result.Row != null)
                {
                    try
                    {
                        // Parse quantity and dates
                        int? quantity = null;
                        if (!string.IsNullOrWhiteSpace(result.Row.Qty) && int.TryParse(result.Row.Qty, out int qty))
                        {
                            quantity = qty;
                        }

                        DateTime? invoiceDate = null;
                        if (!string.IsNullOrWhiteSpace(result.Row.InvoiceDate) && DateTime.TryParse(result.Row.InvoiceDate, out DateTime invDate))
                        {
                            invoiceDate = invDate;
                        }

                        var shipment = new VelocityShipment
                        {
                            DistributorIdStr = result.Row.CustomerNumber, // Store customer number
                            Sku = result.Row.ProductNumber, // Product number as SKU
                            Quantity = quantity,
                            ShippedAt = invoiceDate,
                            Origin = result.Row.City,
                            Destination = result.Row.CustomerName,
                            ManifestLine = JsonSerializer.Serialize(result.Row), // Store all data as JSON
                            JobId = job.Id,
                            IngestedAt = DateTime.UtcNow
                        };

                        shipmentBatch.Add(shipment);

                        jobRow.Status = "success";
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        jobRow.Status = "failed";
                        jobRow.ErrorMessage = ex.Message;
                        failedCount++;

                        _logger.LogError(ex, "Job {JobId} Row {RowIndex}: Failed to create shipment",
                            job.JobId, rowIndex);
                    }
                }
                else
                {
                    jobRow.Status = "failed";
                    jobRow.ErrorMessage = string.Join("; ", result.Errors);
                    failedCount++;
                }

                jobRowBatch.Add(jobRow);

                // Save batch when it reaches batchSize or at the end
                if (jobRowBatch.Count >= batchSize || i == validationResults.Count - 1)
                {
                    try
                    {
                        // Save shipments and job rows in batch
                        if (shipmentBatch.Count > 0)
                        {
                            await _repository.CreateShipmentsBatchAsync(shipmentBatch);
                            _logger.LogInformation("Job {JobId}: Saved batch of {Count} shipments", job.JobId, shipmentBatch.Count);
                        }

                        await _repository.CreateJobRowsBatchAsync(jobRowBatch);
                        _logger.LogInformation("Job {JobId}: Saved batch of {Count} job rows", job.JobId, jobRowBatch.Count);

                        // Update job progress
                        job.ProcessedRows = i + 1;
                        await _repository.UpdateJobAsync(job);

                        // Clear batches
                        shipmentBatch.Clear();
                        jobRowBatch.Clear();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Job {JobId}: Failed to save batch at row {RowIndex}", job.JobId, i);
                        throw;
                    }
                }
            }

                // Update job status
                job.ProcessedRows = validationResults.Count;
                job.SuccessRows = successCount;
                job.FailedRows = failedCount;
                job.CompletedAt = DateTime.UtcNow;

                if (failedCount == 0)
                {
                    job.Status = "completed";
                }
                else if (successCount == 0)
                {
                    job.Status = "failed";
                    job.ErrorMessage = "All rows failed validation or processing";
                }
                else
                {
                    job.Status = "completed_with_errors";
                    job.ErrorMessage = $"{failedCount} of {validationResults.Count} rows failed";
                }

                await _repository.UpdateJobAsync(job);

                _logger.LogInformation("Job {JobId} completed: {SuccessRows} success, {FailedRows} failed",
                    job.JobId, successCount, failedCount);

                // Send email notification
                await SendJobCompletionEmailAsync(job, createdBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}", job.JobId);
                job.Status = "failed";
                job.ErrorMessage = $"Processing error: {ex.Message}";
                job.CompletedAt = DateTime.UtcNow;
                await _repository.UpdateJobAsync(job);

                // Send failure email notification
                await SendJobCompletionEmailAsync(job, createdBy);

                throw;
            }
        }

        private async Task SendJobCompletionEmailAsync(VelocityJob job, string userEmail)
        {
            try
            {
                var baseUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4201";
                var detailsUrl = $"{baseUrl}/admin/velocity/jobs/{job.JobId}";

                await _emailService.SendVelocityJobCompletionEmailAsync(
                    userEmail,
                    userEmail.Split('@')[0], // Use email prefix as name
                    job.JobId,
                    job.TotalRows,
                    job.SuccessRows,
                    job.FailedRows,
                    job.Status,
                    detailsUrl
                );

                _logger.LogInformation("Sent completion email for job {JobId} to {Email}", job.JobId, userEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send completion email for job {JobId}", job.JobId);
                // Don't throw - email failure shouldn't fail the job
            }
        }

        public async Task ResumeJobAsync(int jobId, string createdBy)
        {
            _logger.LogInformation("Resuming job {JobId}", jobId);
            await ProcessCsvDataAsync(jobId, createdBy);
        }

        public async Task<VelocityIngestResponse> RestartJobAsync(string jobId)
        {
            var job = await _repository.GetJobByJobIdAsync(jobId);
            if (job == null)
            {
                return new VelocityIngestResponse
                {
                    JobId = jobId,
                    Status = "error",
                    Message = "Job not found"
                };
            }

            // Check if job can be restarted
            if (job.Status == "processing")
            {
                return new VelocityIngestResponse
                {
                    JobId = jobId,
                    Status = "error",
                    Message = "Job is currently processing. Cannot restart."
                };
            }

            // Get job data
            var jobData = await _repository.GetJobDataAsync(job.Id);
            if (jobData == null)
            {
                return new VelocityIngestResponse
                {
                    JobId = jobId,
                    Status = "error",
                    Message = "Job data not found. Cannot restart. Please re-upload the file."
                };
            }

            // Reset job status
            job.Status = "queued";
            job.ErrorMessage = null;
            job.StartedAt = null;
            job.CompletedAt = null;
            job.ProcessedRows = 0;
            job.SuccessRows = 0;
            job.FailedRows = 0;
            await _repository.UpdateJobAsync(job);

            // Start background processing in a new DI scope
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IVelocityService>();
                    _logger.LogInformation("Restarting job {JobId}", job.JobId);
                    await service.ResumeJobAsync(job.Id, jobData.CreatedBy);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error restarting job {JobId}", job.JobId);
                }
            });

            var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4201";
            var detailsUrl = $"{frontendUrl}/admin/velocity/jobs/{job.JobId}";

            return new VelocityIngestResponse
            {
                JobId = job.JobId,
                Status = "queued",
                Message = "Job restarted successfully",
                DetailsUrl = detailsUrl
            };
        }
    }
}

