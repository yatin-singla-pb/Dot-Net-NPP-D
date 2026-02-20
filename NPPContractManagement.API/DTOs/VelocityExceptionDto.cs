namespace NPPContractManagement.API.DTOs
{
    /// <summary>
    /// DTO for Velocity Exception (failed velocity job rows)
    /// </summary>
    public class VelocityExceptionDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobIdStr { get; set; } = string.Empty;
        public int RowIndex { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? RawData { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string? FileName { get; set; }
        public string? CreatedBy { get; set; }

        // Parsed data from RawData JSON (if available)
        public string? OpCo { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? InvoiceDate { get; set; }
        public string? ProductNumber { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? ManufacturerName { get; set; }
        public string? Qty { get; set; }

        // Action tracking
        public string? ActionStatus { get; set; }
        public string? ActionNotes { get; set; }
        public string? ActionTakenBy { get; set; }
        public DateTime? ActionTakenAt { get; set; }
    }

    /// <summary>
    /// Request to perform an action on a velocity exception
    /// </summary>
    public class VelocityExceptionActionRequest
    {
        public string Action { get; set; } = string.Empty; // Dismissed, NewContract, Amendment
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request DTO for Velocity Exceptions Report
    /// </summary>
    public class VelocityExceptionsRequest
    {
        /// <summary>
        /// Filter by job ID
        /// </summary>
        public int? JobId { get; set; }

        /// <summary>
        /// Filter by date range (start)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter by date range (end)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Search keyword (searches error message, raw data)
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Page number
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Response DTO for Velocity Exceptions Report
    /// </summary>
    public class VelocityExceptionsResponse
    {
        public List<VelocityExceptionDto> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

