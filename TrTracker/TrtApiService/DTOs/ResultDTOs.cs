using System.ComponentModel.DataAnnotations;

namespace TrtApiService.DTOs
{
    /// <summary>
    /// Create operations DTO
    /// </summary>
    public class CreateResultDTO
    {
        [Required] // Let it be string at first, then i need my own enum
        public string Outcome { get; set; } = string.Empty;

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public string? ErrType { get; set; }
        public string? ErrMsg { get; set; }
        public string? ErrStack { get; set; }
        public string? StdOut { get; set; }
        public string? StdErr { get; set; }

        // FK
        [Required] public int TestrunId { get; set; }
        [Required] public int TestId { get; set; }
    }

    /// <summary>
    /// Update operations DTO
    /// </summary>
    public class UpdateResultDTO
    {
        [Required] // Let it be string at first, then i need my own enum
        public string Outcome { get; set; } = string.Empty;

        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }

        public string? ErrType { get; set; }
        public string? ErrMsg { get; set; }
        public string? ErrStack { get; set; }
        public string? StdOut { get; set; }
        public string? StdErr { get; set; }
    }
}
