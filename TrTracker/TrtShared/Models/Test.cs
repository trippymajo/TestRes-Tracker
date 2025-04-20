using System.ComponentModel.DataAnnotations;

namespace TR_Tracker.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
