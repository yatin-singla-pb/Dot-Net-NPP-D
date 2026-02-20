namespace NPPContractManagement.API.DTOs
{
    public class SftpProbeConfigDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 22;
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; } // Never return actual password
        public bool HasPassword { get; set; }
        public bool HasPrivateKey { get; set; }
        public string RemotePath { get; set; } = "/";
        public string? FilePattern { get; set; }
        public bool IsActive { get; set; }
        public int IntervalMinutes { get; set; }
        public DateTime? LastProbeAt { get; set; }
        public DateTime? LastSuccessAt { get; set; }
        public string? LastError { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateSftpProbeConfigDto
    {
        public string Name { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 22;
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? PrivateKey { get; set; }
        public string RemotePath { get; set; } = "/";
        public string? FilePattern { get; set; } = "*.csv";
        public bool IsActive { get; set; } = true;
        public int IntervalMinutes { get; set; } = 60;
    }

    public class UpdateSftpProbeConfigDto
    {
        public string? Name { get; set; }
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? PrivateKey { get; set; }
        public string? RemotePath { get; set; }
        public string? FilePattern { get; set; }
        public bool? IsActive { get; set; }
        public int? IntervalMinutes { get; set; }
    }
}

