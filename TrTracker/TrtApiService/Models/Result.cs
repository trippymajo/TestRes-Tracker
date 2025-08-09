using System.ComponentModel.DataAnnotations;

namespace TrtApiService.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        [Required] // Let it be string at first, then i need my own enum
        public string Outcome { get; set; } = string.Empty;

        // Time
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }
        public long? DurationMs { get; set; }


        // Error logging
        public string? ErrType { get; set; }
        public string? ErrMsg { get; set; }
        public string? ErrStack { get; set; }
        public string? StdOut { get; set; }
        public string? StdErr { get; set; }

        // Foreign Key
        public int TestrunId { get; set; }
        public Testrun Testrun { get; set; } = null!;

        // Foreign Key
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
    }
}
