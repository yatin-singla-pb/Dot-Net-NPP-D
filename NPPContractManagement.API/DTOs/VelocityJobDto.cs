namespace NPPContractManagement.API.DTOs
{
    public class VelocityJobDto
    {
        public int Id { get; set; }
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? SftpFileUrl { get; set; }
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ErrorMessage { get; set; }
        public string? DetailsUrl { get; set; }
    }

    public class VelocityJobDetailDto : VelocityJobDto
    {
        public List<VelocityJobRowDto> Rows { get; set; } = new();
    }

    public class VelocityJobRowDto
    {
        public int Id { get; set; }
        public int RowIndex { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? RawData { get; set; }
        public int? VelocityShipmentId { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class VelocityIngestRequest
    {
        public string? SftpFileUrl { get; set; }
        public Dictionary<string, string>? JobMeta { get; set; }
    }

    public class VelocityIngestResponse
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? DetailsUrl { get; set; }
        public string? Message { get; set; }
    }
}

