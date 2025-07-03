using System.ComponentModel.DataAnnotations;

namespace TrtApiService.Models
{
    public class Testrun
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Version { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)] // TODO: need to think globally
        public DateTime Date { get; set; }

        // Foreign Key
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        // Navigation
        public ICollection<Result> Results { get; set; } = new HashSet<Result>();
    }
}
