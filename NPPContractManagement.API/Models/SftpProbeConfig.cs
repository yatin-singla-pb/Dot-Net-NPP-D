using System.ComponentModel.DataAnnotations;

namespace NPPContractManagement.API.Models
{
    /// <summary>
    /// Configuration for sFTP probe jobs
    /// </summary>
    public class SftpProbeConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Host { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; } = 22;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Password { get; set; } // Encrypted

        [MaxLength(4000)]
        public string? PrivateKey { get; set; } // Encrypted

        [Required]
        [MaxLength(500)]
        public string RemotePath { get; set; } = "/";

        [MaxLength(100)]
        public string? FilePattern { get; set; } = "*.csv";

        public bool IsActive { get; set; } = true;

        public int IntervalMinutes { get; set; } = 60; // How often to probe

        public DateTime? LastProbeAt { get; set; }
        public DateTime? LastSuccessAt { get; set; }

        [MaxLength(1000)]
        public string? LastError { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
    }
}

