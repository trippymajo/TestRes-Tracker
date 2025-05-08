using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TrtShared.ServiceCommunication;
using TrtUploadService.UploadResultsService;
using TrtUploadService.UploadService;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : Controller
    {
        private static readonly HashSet<string> AllowedExtensions = [".trx"];
        private readonly IUploadDocService _uploadDoc;
        private readonly IUploadResultsService _uploadResults;
        private readonly IUploadTransport _resultTransport;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IUploadDocService uploadDoc, IUploadResultsService uploadResults, IUploadTransport resultTransport, ILogger<UploadController> logger)
        {
            _uploadDoc = uploadDoc;
            _uploadResults = uploadResults;
            _resultTransport = resultTransport;
            _logger = logger;
        }

        /// <summary>
        /// POST file to be saved, parsed and pushed to DB
        /// </summary>
        /// <param name="file">File to save</param>
        /// <returns>Http result</returns>
        [HttpPost]
        public async Task<IActionResult> UploadDoc(IFormFile file)
        {
            if (file == null)
            {
                _logger.LogWarning("Uploading doc failed! Incoming file is nul");
                return BadRequest("File is null");
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
                return BadRequest("Unsuported file extension!");
            }

            var fullFilePath = await _uploadDoc.SaveFileAsync(file);
            if (fullFilePath == null)
            {
                _logger.LogWarning("Uploading doc failed! Error on saving file to local server");
                return BadRequest("Error on saving file to local server");
            }

            // Task to subscribe, in order to prevent race
            var dtoTask = _resultTransport.GetParsedDtoAsync(TimeSpan.FromSeconds(60));

            // Now publishing path to ParserService
            await _resultTransport.PublishPathToFileAsync(fullFilePath);
            _logger.LogInformation("File has been successfully uploaded!");

            // Waiting for the response from ParserService
            var parsedDto = await dtoTask;
            if (parsedDto == null)
                return StatusCode(500, "ParserService returned null or failed");

            var result = await _uploadResults.PushResultsToDb(parsedDto);
            if (!result)
                return StatusCode(500, "Failed to save TestRun to database");

            return Ok("Added to DB");
        }
    }
}
