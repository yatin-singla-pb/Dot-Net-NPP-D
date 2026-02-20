namespace NPPContractManagement.API.DTOs
{
    /// <summary>
    /// Request DTO for Velocity Usage Report
    /// </summary>
    public class VelocityUsageReportRequest
    {
        /// <summary>
        /// Start date for the report (default: 30 days ago)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date for the report (default: today)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Filter by keyword (searches product description, brand, manufacturer)
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Filter by manufacturer IDs (can select multiple)
        /// </summary>
        public List<int>? ManufacturerIds { get; set; }

        /// <summary>
        /// Filter by Op-Co IDs
        /// </summary>
        public List<int>? OpCoIds { get; set; }

        /// <summary>
        /// Filter by Industry IDs
        /// </summary>
        public List<int>? IndustryIds { get; set; }

        /// <summary>
        /// Page number for pagination
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size for pagination
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Sort field
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction (asc/desc)
        /// </summary>
        public string? SortDirection { get; set; }
    }

    /// <summary>
    /// Response DTO for Velocity Usage Report (aggregated data)
    /// </summary>
    public class VelocityUsageReportResponse
    {
        public List<VelocityUsageAggregateDto> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// Aggregated velocity usage data by product
    /// </summary>
    public class VelocityUsageAggregateDto
    {
        public string? Manufacturer { get; set; }
        public string? Product { get; set; }
        public int CasesPurchased { get; set; }
        public DateTime? MinShipmentDate { get; set; }
        public DateTime? MaxShipmentDate { get; set; }
        public string? ProductId { get; set; }
        public string? DistributorProductCode { get; set; }
        public string? Brand { get; set; }
        public string? PackSize { get; set; }
        public string? ProductDescription { get; set; }
        public string? MfrProductCode { get; set; }
        public string? GTIN { get; set; }
        public decimal? AvgLandedCost { get; set; }

        /// <summary>
        /// Unique key for grouping (combination of product identifiers)
        /// </summary>
        public string GroupKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Individual velocity record detail
    /// </summary>
    public class VelocityUsageDetailDto
    {
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public string? DistributorProductCode { get; set; }
        public string? Brand { get; set; }
        public string? PackSize { get; set; }
        public string? ProductDescription { get; set; }
        public string? MfrProductCode { get; set; }
        public string? GTIN { get; set; }
        public string? Manufacturer { get; set; }
        public int? CasesPurchased { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? ContractPrice { get; set; }
        public decimal? LandedCost { get; set; }
        public string? Category { get; set; }
        public string? SecondaryCategory { get; set; }
        public string? TertiaryCategory { get; set; }
        public string? OpCo { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerName { get; set; }
    }

    /// <summary>
    /// Request to create proposal from velocity data
    /// </summary>
    public class CreateProposalFromVelocityRequest
    {
        /// <summary>
        /// Selected group keys from the aggregated report
        /// </summary>
        public List<string> SelectedGroupKeys { get; set; } = new();

        /// <summary>
        /// Percentage increase/decrease for quantity estimation
        /// </summary>
        public decimal? QuantityAdjustmentPercent { get; set; }

        /// <summary>
        /// Minimum quantity threshold to apply adjustment
        /// </summary>
        public int? MinimumQuantityThreshold { get; set; }

        /// <summary>
        /// Proposed due date for the proposal
        /// </summary>
        public DateTime? ProposalDueDate { get; set; }

        /// <summary>
        /// Proposal title
        /// </summary>
        public string? ProposalTitle { get; set; }
    }
}

