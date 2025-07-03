using System.ComponentModel.DataAnnotations;

namespace TrtApiService.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Testrun> Testruns { get; set; } = new HashSet<Testrun>();
    }
}
