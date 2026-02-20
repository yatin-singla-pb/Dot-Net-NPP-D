namespace NPPContractManagement.API.DTOs
{
    /// <summary>
    /// Request to create multiple proposal requests from existing contracts
    /// </summary>
    public class BulkRenewalRequest
    {
        /// <summary>
        /// List of contract IDs to renew
        /// </summary>
        public List<int> ContractIds { get; set; } = new();

        /// <summary>
        /// Pricing adjustment configuration
        /// </summary>
        public PricingAdjustment? PricingAdjustment { get; set; }

        /// <summary>
        /// Suggested due date for all proposals
        /// </summary>
        public DateTime? ProposalDueDate { get; set; }

        /// <summary>
        /// Additional product IDs to add to all proposals (optional)
        /// </summary>
        public List<int> AdditionalProductIds { get; set; } = new();

        /// <summary>
        /// User creating the renewal requests
        /// </summary>
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Pricing adjustment configuration
    /// </summary>
    public class PricingAdjustment
    {
        /// <summary>
        /// Percentage increase or decrease (e.g., 5.0 for 5% increase, -3.0 for 3% decrease)
        /// </summary>
        public decimal PercentageChange { get; set; }

        /// <summary>
        /// Minimum quantity threshold to apply the adjustment
        /// </summary>
        public int? MinimumQuantityThreshold { get; set; }

        /// <summary>
        /// Whether to apply adjustment to all products or only those meeting threshold
        /// </summary>
        public bool ApplyToAllProducts { get; set; } = true;
    }

    /// <summary>
    /// Response from bulk renewal operation
    /// </summary>
    public class BulkRenewalResponse
    {
        /// <summary>
        /// Total number of contracts processed
        /// </summary>
        public int TotalContracts { get; set; }

        /// <summary>
        /// Number of proposals successfully created
        /// </summary>
        public int SuccessfulProposals { get; set; }

        /// <summary>
        /// Number of proposals that failed
        /// </summary>
        public int FailedProposals { get; set; }

        /// <summary>
        /// List of created proposal IDs
        /// </summary>
        public List<int> CreatedProposalIds { get; set; } = new();

        /// <summary>
        /// Detailed results for each contract
        /// </summary>
        public List<ContractRenewalResult> Results { get; set; } = new();

        /// <summary>
        /// Overall success status
        /// </summary>
        public bool Success => FailedProposals == 0;

        /// <summary>
        /// Summary message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result for a single contract renewal
    /// </summary>
    public class ContractRenewalResult
    {
        /// <summary>
        /// Source contract ID
        /// </summary>
        public int ContractId { get; set; }

        /// <summary>
        /// Source contract number
        /// </summary>
        public string? ContractNumber { get; set; }

        /// <summary>
        /// Whether the proposal was created successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Created proposal ID (if successful)
        /// </summary>
        public int? ProposalId { get; set; }

        /// <summary>
        /// Error message (if failed)
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Number of products added to the proposal
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Number of additional products added
        /// </summary>
        public int AdditionalProductCount { get; set; }
    }

    /// <summary>
    /// Contract summary for bulk renewal selection
    /// </summary>
    public class ContractRenewalSummary
    {
        public int Id { get; set; }
        public string? ContractNumber { get; set; }
        public string? ManufacturerName { get; set; }
        public string? DistributorName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ActiveProductCount { get; set; }
        public int DiscontinuedProductCount { get; set; }
        public bool CanRenew { get; set; }
        public string? RenewalBlockReason { get; set; }
    }
}

