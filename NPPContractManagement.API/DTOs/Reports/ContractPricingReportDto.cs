namespace NPPContractManagement.API.DTOs.Reports
{
    /// <summary>
    /// Request for Contract Pricing Report
    /// </summary>
    public class ContractPricingReportRequest
    {
        /// <summary>
        /// Filter by contract number (ForeignContractId)
        /// </summary>
        public string? ContractNumber { get; set; }

        /// <summary>
        /// Filter by manufacturer ID
        /// </summary>
        public int? ManufacturerId { get; set; }

        /// <summary>
        /// Filter by product ID
        /// </summary>
        public int? ProductId { get; set; }

        /// <summary>
        /// Filter by OpCo ID
        /// </summary>
        public int? OpCoId { get; set; }

        /// <summary>
        /// Filter by Industry ID
        /// </summary>
        public int? IndustryId { get; set; }

        /// <summary>
        /// Filter by contract name
        /// </summary>
        public string? ContractName { get; set; }

        /// <summary>
        /// Filter by distributor ID
        /// </summary>
        public int? DistributorId { get; set; }

        /// <summary>
        /// Filter by contract start date (from)
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Filter by contract start date (to)
        /// </summary>
        public DateTime? StartDateTo { get; set; }

        /// <summary>
        /// Filter by contract end date (from)
        /// </summary>
        public DateTime? EndDateFrom { get; set; }

        /// <summary>
        /// Filter by contract end date (to)
        /// </summary>
        public DateTime? EndDateTo { get; set; }

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
    /// Response for Contract Pricing Report
    /// </summary>
    public class ContractPricingReportResponse
    {
        public List<ContractPricingReportRow> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// Single row in the Contract Pricing Report
    /// </summary>
    public class ContractPricingReportRow
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OpCos { get; set; } = string.Empty; // Comma-separated
        public string? Industry { get; set; }
        public int ContractVersionNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int PricingVersionNumber { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? CommercialDelivered { get; set; }
        public decimal? CommodityDelivered { get; set; }
        public decimal? CommercialFOB { get; set; }
        public decimal? CommodityFOB { get; set; }
        public string UOM { get; set; } = string.Empty;
        public decimal? EstimatedVolume { get; set; }
        public decimal? ActualVolume { get; set; }
    }
}

