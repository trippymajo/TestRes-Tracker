using Microsoft.AspNetCore.Mvc;
using TrtApiService.App.UploadParsedService;
using TrtShared.DTO;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadResultsController : ControllerBase
    {
        private readonly IUploadParsedService _uploadService;
        private readonly ILogger _logger;

        // Default Constructor
        public UploadResultsController(IUploadParsedService uploadService, 
            ILogger<UploadResultsController> logger)
        {
            _logger = logger;
            _uploadService = uploadService;
        }

        /// <summary>
        /// POST: api/UploadResults
        /// POSTs all info about test run in to the DB
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Http result of the operation</returns>
        [HttpPost]
        public async Task<IActionResult> UploadResultsAsync([FromBody] TestRunDTO dto)
        {
            if (dto == null 
                || dto.Results.Any() == false 
                || string.IsNullOrWhiteSpace(dto.Branch)
                || string.IsNullOrWhiteSpace(dto.Version))
            {
                _logger.LogError("Results data is invalid");
                return BadRequest("Invalid data");
            }

            if (await _uploadService.UploadParsedAsync(dto) == false)
                return StatusCode(500, "Failed to process testRun upload");

            return Ok("New results been successfully uploaded");
        }
    }
}
