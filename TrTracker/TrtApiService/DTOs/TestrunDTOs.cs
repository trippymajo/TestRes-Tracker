using System.ComponentModel.DataAnnotations;


namespace TrtApiService.DTOs
{
    /// <summary>
    /// C - Create, U - Update.
    /// </summary>
    public class CUTestrunDTO
    {
        [Required]
        public string Version { get; set; } = string.Empty;

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public string? IdempotencyKey { get; set; }
        public string? EnvironmentJson { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}
