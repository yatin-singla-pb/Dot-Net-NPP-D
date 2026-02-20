using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ForeignContractId { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? InternalNotes { get; set; }
        // Manufacturer/Entegra metadata
        public string? ManufacturerReferenceNumber { get; set; }
        public string? ManufacturerBillbackName { get; set; }
        public string? ManufacturerTermsAndConditions { get; set; }
        public string? ManufacturerNotes { get; set; }
        public string? ContactPerson { get; set; }
        public string? EntegraContractType { get; set; }
        public string? EntegraVdaProgram { get; set; }
        public bool IsSuspended { get; set; }
        public bool SendToPerformance { get; set; }
        public int CurrentVersionNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public int? ProposalId { get; set; }
        public string? ProposalTitle { get; set; }

        // Legacy fields for backward compatibility (populated from first manufacturer in list)
        public int? ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; }

        public List<DistributorDto> Distributors { get; set; } = new List<DistributorDto>();
        public List<OpCoDto> OpCos { get; set; } = new List<OpCoDto>();
        public List<IndustryDto> Industries { get; set; } = new List<IndustryDto>();
        public List<ManufacturerDto> Manufacturers { get; set; } = new List<ManufacturerDto>();
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }

    public class CreateContractDto : IValidatableObject
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? ForeignContractId { get; set; }

        // Removed: SuspendedDate is system-managed via Suspend/Unsuspend actions

        [Required]
        // Manufacturer/Entegra metadata
        [StringLength(100)]
        public string? ManufacturerReferenceNumber { get; set; }
        [StringLength(200)]
        public string? ManufacturerBillbackName { get; set; }
        public string? ManufacturerTermsAndConditions { get; set; }
        [StringLength(2000)]
        public string? ManufacturerNotes { get; set; }
        [StringLength(200)]
        public string? ContactPerson { get; set; }
        [StringLength(100)]
        public string? EntegraContractType { get; set; }
        [StringLength(100)]
        public string? EntegraVdaProgram { get; set; }

        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(2000)]
        public string? InternalNotes { get; set; }

        // Removed: IsSuspended is system-managed via Suspend/Unsuspend actions

        public int? ProposalId { get; set; }


        public bool SendToPerformance { get; set; } = false;

        [Required]
        public List<int> DistributorIds { get; set; } = new List<int>();

        public List<int> OpCoIds { get; set; } = new List<int>();

        public List<int> IndustryIds { get; set; } = new List<int>();

        public List<int> ProductIds { get; set; } = new List<int>();

        public List<CreateContractPriceRequest> Prices { get; set; } = new List<CreateContractPriceRequest>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
                yield return new ValidationResult("EndDate must be on or after StartDate", new[] { nameof(EndDate) });

            // Validate EntegraContractType if provided
            if (!string.IsNullOrWhiteSpace(EntegraContractType))
            {
                var validTypes = new[] { "FOP", "GAA", "GPP", "MKT", "USG", "VDA" };
                if (!validTypes.Contains(EntegraContractType, StringComparer.OrdinalIgnoreCase))
                {
                    yield return new ValidationResult(
                        $"EntegraContractType must be one of: {string.Join(", ", validTypes)}",
                        new[] { nameof(EntegraContractType) });
                }
            }

            // Removed: SuspendedDate/IsSuspended validation; managed by server actions
        }
    }

    public class UpdateContractDto : IValidatableObject
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? ForeignContractId { get; set; }

        // Removed: SuspendedDate is system-managed via Suspend/Unsuspend actions

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [StringLength(2000)]
        public string? InternalNotes { get; set; }


	        // Manufacturer/Entegra metadata
	        [StringLength(100)]
	        public string? ManufacturerReferenceNumber { get; set; }
	        [StringLength(200)]
	        public string? ManufacturerBillbackName { get; set; }
	        public string? ManufacturerTermsAndConditions { get; set; }
	        [StringLength(2000)]
	        public string? ManufacturerNotes { get; set; }
	        [StringLength(200)]
	        public string? ContactPerson { get; set; }
	        [StringLength(100)]
	        public string? EntegraContractType { get; set; }
	        [StringLength(100)]
	        public string? EntegraVdaProgram { get; set; }

        // Removed: IsSuspended is system-managed via Suspend/Unsuspend actions

        public bool SendToPerformance { get; set; }

        [Required]
        public List<int> DistributorIds { get; set; } = new List<int>();

        public List<int> OpCoIds { get; set; } = new List<int>();

        [Required]
        public List<int> IndustryIds { get; set; } = new List<int>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
                yield return new ValidationResult("EndDate must be on or after StartDate", new[] { nameof(EndDate) });

            // Validate EntegraContractType if provided
            if (!string.IsNullOrWhiteSpace(EntegraContractType))
            {
                var validTypes = new[] { "FOP", "GAA", "GPP", "MKT", "USG", "VDA" };
                if (!validTypes.Contains(EntegraContractType, StringComparer.OrdinalIgnoreCase))
                {
                    yield return new ValidationResult(
                        $"EntegraContractType must be one of: {string.Join(", ", validTypes)}",
                        new[] { nameof(EntegraContractType) });
                }
            }

            // Removed: SuspendedDate/IsSuspended validation; managed by server actions
            yield break;
        }
    }

    public class IndustryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateIndustryDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int Status { get; set; } = 1; // Active by default
    }

    public class UpdateIndustryDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int TotalIndustries { get; set; }
        public int TotalDistributors { get; set; }
        public int TotalOpCos { get; set; }
        public int TotalManufacturers { get; set; }
    }
}
