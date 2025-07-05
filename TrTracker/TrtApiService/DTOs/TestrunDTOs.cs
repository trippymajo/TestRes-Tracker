using System.ComponentModel.DataAnnotations;

namespace TrtApiService.DTOs
{
    public class UpdateTestrunDTO
    {
        [Required]
        public string Version { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int BranchId { get; set; }
    }

    public class CreateTestrunDTO
    {
        [Required]
        public string Version { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}
