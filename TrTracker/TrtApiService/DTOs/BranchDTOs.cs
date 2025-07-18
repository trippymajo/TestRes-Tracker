﻿using System.ComponentModel.DataAnnotations;

namespace TrtApiService.DTOs
{
    /// <summary>
    /// C - Create, U - Update.
    /// </summary>
    public class CUBranchDTO
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }
}
