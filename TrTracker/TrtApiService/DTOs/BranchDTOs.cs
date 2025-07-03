using System.ComponentModel.DataAnnotations;

namespace TrtApiService.DTOs
{
    public class UpdateBranchDTO
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateBranchDTO
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set;} = string.Empty;
    }
}
