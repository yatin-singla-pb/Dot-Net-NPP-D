using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class ContractDistributorAssignmentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int DistributorId { get; set; }
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class CreateContractDistributorAssignmentRequest
    {
        [Required]
        public int DistributorId { get; set; }
        [Required]
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class ContractManufacturerAssignmentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int ManufacturerId { get; set; }
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class CreateContractManufacturerAssignmentRequest
    {
        [Required]
        public int ManufacturerId { get; set; }
        [Required]
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class ContractOpCoAssignmentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int OpCoId { get; set; }
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class CreateContractOpCoAssignmentRequest
    {
        [Required]
        public int OpCoId { get; set; }
        [Required]
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class ContractIndustryAssignmentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int IndustryId { get; set; }
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }

    public class CreateContractIndustryAssignmentRequest
    {
        [Required]
        public int IndustryId { get; set; }
        [Required]
        public int CurrentVersionNumber { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
}

