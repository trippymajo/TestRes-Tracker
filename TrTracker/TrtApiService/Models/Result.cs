using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrtApiService.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        [Required] // Let it be string at first, then i need my own enum
        public string Outcome { get; set; } = string.Empty;
        public string? ErrMsg { get; set; }

        // Foreign Key
        public int TestrunId { get; set; }
        public Testrun Testrun { get; set; } = null!;

        // Foreign Key
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
    }
}
