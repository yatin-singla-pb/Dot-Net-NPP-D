namespace NPPContractManagement.API.DTOs.Reports
{
    /// <summary>
    /// Request for Contract Over Term Report
    /// </summary>
    public class ContractOverTermReportRequest
    {
        /// <summary>
        /// Point in time to start from (e.g., today)
        /// </summary>
        public DateTime PointInTime { get; set; } = DateTime.Today;

        /// <summary>
        /// Maximum number of previous contract terms to include
        /// </summary>
        public int MaxTermsBack { get; set; } = 3;

        /// <summary>
        /// Optional filter by contract number
        /// </summary>
        public string? ContractNumber { get; set; }

        /// <summary>
        /// Optional filter by manufacturer ID
        /// </summary>
        public int? ManufacturerId { get; set; }

        /// <summary>
        /// Optional filter by product ID
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// Optional filter by OpCo ID
        /// </summary>
        public int? OpCoId { get; set; }

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Response for Contract Over Term Report
    /// </summary>
    public class ContractOverTermReportResponse
    {
        public List<ContractOverTermReportRow> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public int MaxTermsBack { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// Single row in the Contract Over Term Report
    /// </summary>
    public class ContractOverTermReportRow
    {
        // Current Term
        public string ContractNumber { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OpCos { get; set; } = string.Empty; // Comma-separated
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal? Pricing { get; set; }
        public decimal? EstimatedVolume { get; set; }
        public decimal? ActualVolume { get; set; }
        public string? Industry { get; set; }
        public string? PriceType { get; set; }

        // Previous Terms (dynamic list)
        public List<PreviousTermData> PreviousTerms { get; set; } = new();
    }

    /// <summary>
    /// Data for a previous contract term
    /// </summary>
    public class PreviousTermData
    {
        public int TermNumber { get; set; } // 1, 2, 3, etc.
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Pricing { get; set; }
        public decimal? EstimatedVolume { get; set; }
        public decimal? ActualVolume { get; set; }
    }
}

