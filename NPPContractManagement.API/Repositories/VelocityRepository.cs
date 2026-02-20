using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.DTOs;
using System.Text.Json;

namespace NPPContractManagement.API.Repositories
{
    public class VelocityRepository : IVelocityRepository
    {
        private readonly ApplicationDbContext _context;

        public VelocityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // IngestedFile operations
        public async Task<IngestedFile> CreateIngestedFileAsync(IngestedFile file)
        {
            _context.IngestedFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<IngestedFile?> GetIngestedFileByIdAsync(int id)
        {
            return await _context.IngestedFiles.FindAsync(id);
        }

        // Job operations
        public async Task<VelocityJob> CreateJobAsync(VelocityJob job)
        {
            _context.VelocityJobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<VelocityJob?> GetJobByJobIdAsync(string jobId)
        {
            // JobId is a computed property (Id.ToString()), so we need to parse it back to int
            if (int.TryParse(jobId, out int id))
            {
                return await _context.VelocityJobs
                    .Include(j => j.JobRows)
                    .FirstOrDefaultAsync(j => j.Id == id);
            }
            return null;
        }

        public async Task<VelocityJob?> GetJobByIdAsync(int id)
        {
            return await _context.VelocityJobs
                .Include(j => j.JobRows)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task UpdateJobAsync(VelocityJob job)
        {
            _context.VelocityJobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VelocityJob>> GetJobsAsync(int page, int pageSize, VelocityJobStatus? status = null)
        {
            var query = _context.VelocityJobs.AsQueryable();

            if (status.HasValue)
            {
                var statusStr = status.Value.ToString().ToLowerInvariant();
                query = query.Where(j => j.Status == statusStr);
            }

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetJobsCountAsync(VelocityJobStatus? status = null)
        {
            var query = _context.VelocityJobs.AsQueryable();

            if (status.HasValue)
            {
                var statusStr = status.Value.ToString().ToLowerInvariant();
                query = query.Where(j => j.Status == statusStr);
            }

            return await query.CountAsync();
        }

        public async Task<List<VelocityJob>> GetJobsByStatusAsync(string status)
        {
            return await _context.VelocityJobs
                .Where(j => j.Status == status)
                .OrderBy(j => j.CreatedAt)
                .ToListAsync();
        }

        // Job row operations
        public async Task<VelocityJobRow> CreateJobRowAsync(VelocityJobRow row)
        {
            _context.VelocityJobRows.Add(row);
            await _context.SaveChangesAsync();
            return row;
        }

        public async Task CreateJobRowsBatchAsync(List<VelocityJobRow> rows)
        {
            if (rows == null || rows.Count == 0) return;
            _context.VelocityJobRows.AddRange(rows);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VelocityJobRow>> GetJobRowsAsync(int jobId)
        {
            return await _context.VelocityJobRows
                .Where(r => r.JobId == jobId)
                .OrderBy(r => r.RowIndex)
                .ToListAsync();
        }

        public async Task UpdateJobRowAsync(VelocityJobRow row)
        {
            _context.VelocityJobRows.Update(row);
            await _context.SaveChangesAsync();
        }

        // Shipment operations
        public async Task<VelocityShipment> CreateShipmentAsync(VelocityShipment shipment)
        {
            _context.VelocityShipments.Add(shipment);
            await _context.SaveChangesAsync();
            return shipment;
        }

        public async Task CreateShipmentsBatchAsync(List<VelocityShipment> shipments)
        {
            if (shipments == null || shipments.Count == 0) return;
            _context.VelocityShipments.AddRange(shipments);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VelocityShipment>> GetShipmentsAsync(int page, int pageSize, int? distributorId = null, int? jobId = null)
        {
            var query = _context.VelocityShipments
                .Include(s => s.Distributor)
                .AsQueryable();

            if (distributorId.HasValue)
            {
                query = query.Where(s => s.DistributorId == distributorId.Value);
            }

            if (jobId.HasValue)
            {
                query = query.Where(s => s.VelocityJobId == jobId.Value);
            }

            return await query
                .OrderByDescending(s => s.ShippedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetShipmentsCountAsync(int? distributorId = null, int? jobId = null)
        {
            var query = _context.VelocityShipments.AsQueryable();

            if (distributorId.HasValue)
            {
                query = query.Where(s => s.DistributorId == distributorId.Value);
            }

            if (jobId.HasValue)
            {
                query = query.Where(s => s.VelocityJobId == jobId.Value);
            }

            return await query.CountAsync();
        }

        // sFTP config operations
        public async Task<SftpProbeConfig> CreateSftpConfigAsync(SftpProbeConfig config)
        {
            _context.SftpProbeConfigs.Add(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task<SftpProbeConfig?> GetSftpConfigByIdAsync(int id)
        {
            return await _context.SftpProbeConfigs.FindAsync(id);
        }

        public async Task<List<SftpProbeConfig>> GetSftpConfigsAsync()
        {
            return await _context.SftpProbeConfigs
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task UpdateSftpConfigAsync(SftpProbeConfig config)
        {
            _context.SftpProbeConfigs.Update(config);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSftpConfigAsync(int id)
        {
            var config = await _context.SftpProbeConfigs.FindAsync(id);
            if (config != null)
            {
                _context.SftpProbeConfigs.Remove(config);
                await _context.SaveChangesAsync();
            }
        }

        // Job data operations for resume capability
        public async Task StoreJobDataAsync(int jobId, List<VelocityValidationResult> validationResults, string createdBy)
        {
            // Delete existing job data if any
            var existing = await _context.VelocityJobData.FirstOrDefaultAsync(d => d.JobId == jobId);
            if (existing != null)
            {
                _context.VelocityJobData.Remove(existing);
            }

            var jobData = new VelocityJobData
            {
                JobId = jobId,
                ValidationResultsJson = JsonSerializer.Serialize(validationResults),
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.VelocityJobData.Add(jobData);
            await _context.SaveChangesAsync();
        }

        public async Task<VelocityJobData?> GetJobDataAsync(int jobId)
        {
            return await _context.VelocityJobData
                .FirstOrDefaultAsync(d => d.JobId == jobId);
        }

        public async Task DeleteJobDataAsync(int jobId)
        {
            var jobData = await _context.VelocityJobData.FirstOrDefaultAsync(d => d.JobId == jobId);
            if (jobData != null)
            {
                _context.VelocityJobData.Remove(jobData);
                await _context.SaveChangesAsync();
            }
        }

        // Velocity Exceptions operations
        public async Task<(List<VelocityJobRow> Rows, int TotalCount)> GetFailedJobRowsAsync(
            int? jobId,
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.VelocityJobRows
                .Include(r => r.VelocityJob!)
                    .ThenInclude(j => j.IngestedFile!)
                .Where(r => r.Status == "failed");

            // Filter by job ID
            if (jobId.HasValue)
            {
                query = query.Where(r => r.JobId == jobId.Value);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= endDate.Value);
            }

            // Filter by keyword (search in error message and raw data)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(r =>
                    (r.ErrorMessage != null && r.ErrorMessage.Contains(keyword)) ||
                    (r.RawValues != null && r.RawValues.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();

            var rows = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (rows, totalCount);
        }

        // Velocity Usage Report operations
        public async Task<(List<VelocityShipment> Shipments, int TotalCount)> GetVelocityUsageDataAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? keyword,
            List<int>? manufacturerIds,
            List<int>? opCoIds,
            List<int>? industryIds,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDirection)
        {
            var query = _context.VelocityShipments.AsQueryable();

            // Default date range: last 30 days
            var defaultStartDate = DateTime.UtcNow.AddDays(-30);
            var effectiveStartDate = startDate ?? defaultStartDate;
            var effectiveEndDate = endDate ?? DateTime.UtcNow;

            // Filter by shipment date
            query = query.Where(s => s.ShippedAt >= effectiveStartDate && s.ShippedAt <= effectiveEndDate);

            // Filter by keyword (search in manifest_line JSON)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s =>
                    (s.ManifestLine != null && s.ManifestLine.Contains(keyword)) ||
                    (s.Sku != null && s.Sku.Contains(keyword)) ||
                    (s.Origin != null && s.Origin.Contains(keyword)) ||
                    (s.Destination != null && s.Destination.Contains(keyword)));
            }

            // Note: Manufacturer, OpCo, and Industry filtering would require parsing ManifestLine JSON
            // For now, we'll implement basic filtering. Advanced filtering can be added later.

            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDirection);

            var shipments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (shipments, totalCount);
        }

        private IQueryable<VelocityShipment> ApplySorting(
            IQueryable<VelocityShipment> query,
            string? sortBy,
            string? sortDirection)
        {
            var isDescending = sortDirection?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "quantity" => isDescending ? query.OrderByDescending(s => s.Quantity) : query.OrderBy(s => s.Quantity),
                "shippedat" => isDescending ? query.OrderByDescending(s => s.ShippedAt) : query.OrderBy(s => s.ShippedAt),
                "sku" => isDescending ? query.OrderByDescending(s => s.Sku) : query.OrderBy(s => s.Sku),
                _ => query.OrderByDescending(s => s.ShippedAt) // Default sort
            };
        }
    }
}

