using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.DTOs
{
    public class DistributorProductCodeDto
    {
        public int Id { get; set; }
        public int DistributorId { get; set; }
        public string? DistributorName { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ManufacturerProductCode { get; set; }
        public string? Brand { get; set; }
        public string? ManufacturerName { get; set; }
        public string DistributorCode { get; set; } = string.Empty;
        public bool CatchWeight { get; set; }
        public bool EBrand { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateDistributorProductCodeDto
    {
        [Required]
        public int DistributorId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [StringLength(255)]
        public string DistributorCode { get; set; } = string.Empty;
        public bool CatchWeight { get; set; } = false;
        public bool EBrand { get; set; } = false;
    }

    public class UpdateDistributorProductCodeDto
    {
        public int? DistributorId { get; set; }
        public int? ProductId { get; set; }
        [StringLength(255)]
        public string? DistributorCode { get; set; }
        public bool? CatchWeight { get; set; }
        public bool? EBrand { get; set; }
    }

    public class PaginatedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

