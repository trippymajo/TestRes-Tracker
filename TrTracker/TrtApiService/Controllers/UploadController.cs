using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TrtApiService.UploadService;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : Controller
    {
        private static readonly HashSet<string> AllowedExtensions = [".trx"];
        private readonly IConnectionMultiplexer _redis;
        private readonly IUploadService _uploadService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IUploadService uploadService, IConnectionMultiplexer redis, ILogger<UploadController> logger)
        {
            _uploadService = uploadService;
            _redis = redis;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDoc(IFormFile file)
        {
            if (file == null)
            {
                _logger.LogWarning("Uploading doc failed! Incoming file is nul");
                return BadRequest();
            }

            if (file.Length > 50 * 1024 * 1024)
            {
                _logger.LogWarning("Uploading doc failed! Incoming file is more than 50 MB");
                return StatusCode(413);
            }

            var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExt) || !AllowedExtensions.Contains(fileExt))
            {
                _logger.LogWarning("Uploading doc failed! Incoming file's extension is empty");
                return BadRequest();
            }

            var fullFilePath = await _uploadService.SaveFileAsync(file);
            if (fullFilePath == null)
            {
                _logger.LogWarning("Uploading doc failed! Error on saving file to local server");
                return BadRequest();
            }

            try
            {
                var pub = _redis.GetSubscriber();
                var channel = RedisChannel.Literal("file-events");
                await pub.PublishAsync(channel, fullFilePath);
                _logger.LogInformation("Published to Redis: {Path}", fullFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish file path to Redis: {Path}", fullFilePath);
                throw; // Rethrow fatal redis error
            }

            _logger.LogInformation("File has been successfully uploaded!");
            return Ok("Uploaded");
        }
    }
}
