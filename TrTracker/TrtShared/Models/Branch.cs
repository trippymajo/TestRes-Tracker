using System.ComponentModel.DataAnnotations;

namespace TR_Tracker.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        //[Required]
        //public bool IsArchived { get; set; } = false;
    }
}
