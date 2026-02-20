using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.DTOs;
using System.Text.Json;

namespace NPPContractManagement.API.Services
{
    /// <summary>
    /// Background service that auto-resumes incomplete velocity jobs on startup
    /// </summary>
    public class VelocityBackgroundProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VelocityBackgroundProcessor> _logger;
        private bool _hasRunStartupResume = false;

        public VelocityBackgroundProcessor(
            IServiceProvider serviceProvider,
            ILogger<VelocityBackgroundProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Velocity Background Processor started");

            // On startup, resume any incomplete jobs
            if (!_hasRunStartupResume)
            {
                await ResumeIncompleteJobsAsync(stoppingToken);
                _hasRunStartupResume = true;
            }

            // Keep the service running (no polling needed since we use fire-and-forget)
            await Task.Delay(Timeout.Infinite, stoppingToken);

            _logger.LogInformation("Velocity Background Processor stopped");
        }

        private async Task ResumeIncompleteJobsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IVelocityRepository>();

                // Get all jobs that were interrupted (queued or processing)
                var queuedJobs = await repository.GetJobsByStatusAsync("queued");
                var processingJobs = await repository.GetJobsByStatusAsync("processing");
                var incompleteJobs = queuedJobs.Concat(processingJobs).ToList();

                if (incompleteJobs.Count == 0)
                {
                    _logger.LogInformation("No incomplete jobs to resume");
                    return;
                }

                _logger.LogInformation("Found {Count} incomplete jobs to resume", incompleteJobs.Count);

                foreach (var job in incompleteJobs)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    try
                    {
                        _logger.LogInformation("Resuming job {JobId} (status: {Status})", job.JobId, job.Status);

                        // Get job data
                        var jobData = await repository.GetJobDataAsync(job.Id);
                        if (jobData == null)
                        {
                            _logger.LogWarning("Job {JobId}: No job data found. Cannot resume. Marking as failed.", job.JobId);
                            job.Status = "failed";
                            job.ErrorMessage = "Job data not found. Cannot resume after restart.";
                            job.CompletedAt = DateTime.UtcNow;
                            await repository.UpdateJobAsync(job);
                            continue;
                        }

                        // Reset status to queued and restart processing
                        job.Status = "queued";
                        await repository.UpdateJobAsync(job);

                        // Start background processing using fire-and-forget
                        var velocityService = scope.ServiceProvider.GetRequiredService<IVelocityService>();
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                using var innerScope = _serviceProvider.CreateScope();
                                var innerService = innerScope.ServiceProvider.GetRequiredService<IVelocityService>();
                                await innerService.ResumeJobAsync(job.Id, jobData.CreatedBy);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error resuming job {JobId}", job.JobId);
                            }
                        }, stoppingToken);

                        _logger.LogInformation("Job {JobId} queued for resume", job.JobId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error resuming job {JobId}", job.JobId);
                    }
                }

                _logger.LogInformation("Completed startup resume for {Count} jobs", incompleteJobs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResumeIncompleteJobsAsync");
            }
        }
    }
}

