using System.ComponentModel.DataAnnotations;


namespace TrtApiService.DTOs
{
    /// <summary>
    /// C - Create, U - Update.
    /// </summary>
    public class CUTestrunDTO
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
