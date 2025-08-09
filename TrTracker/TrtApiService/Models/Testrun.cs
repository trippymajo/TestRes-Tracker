using System.ComponentModel.DataAnnotations;

namespace TrtApiService.Models
{
    public class Testrun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Version { get; set; } = string.Empty;

        // Time
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public long? DurationMs { get; set; }


        // Agregates
        public int? Total { get; set; }
        public int? Passed { get; set; }
        public int? Failed { get; set; }
        public int? Skipped { get; set; }
        public int? Errors { get; set; }

        // Additional info about environment
        public string? EnvironmentJson { get; set; }

        // HashKey of the current TestRun
        public string? IdempotencyKey { get; set; }

        // Foreign Key
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        // Navigation
        public ICollection<Result> Results { get; set; } = new HashSet<Result>();
    }
}
