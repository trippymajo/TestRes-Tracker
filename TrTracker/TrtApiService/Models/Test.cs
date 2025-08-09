using System.ComponentModel.DataAnnotations;

namespace TrtApiService.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public string? ClassName { get; set; }

        public string? Description { get; set; }

        // Navigation
        public ICollection<Result> Results { get; set; } = new HashSet<Result>();
    }
}
