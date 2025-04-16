using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using TrtApiService.UploadService;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : Controller
    {
        private static readonly HashSet<string> AllowedExtensions = [".trx"];
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDoc(IFormFile file)
        {
            if (file == null) 
                return BadRequest(); 

            if (file.Length > 50 * 1024 * 1024)
                return StatusCode(413);

            var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExt) || !AllowedExtensions.Contains(fileExt))
                return BadRequest();

            if (await _uploadService.SaveFileAsync(file) == null)
                return BadRequest("File was not loaded");

            return Ok("Uploaded");
        }
    }
}
