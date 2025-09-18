using Microsoft.AspNetCore.Mvc;

using TrtApiService.App.UploadParsedService;

using TrtShared.RetValExtensions;
using TrtShared.Envelope;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadParsedController : ControllerBase
    {
        private readonly IUploadParsedService _uploadService;
        private readonly ILogger _logger;

        public UploadParsedController(IUploadParsedService uploadService, 
            ILogger<UploadParsedController> logger)
        {
            _logger = logger;
            _uploadService = uploadService;
        }

        // POST: api/UploadParsed
        [HttpPost]
        public async Task<IActionResult> UploadParsedAsync([FromBody] UniEnvelope dto)
        {
            // Validation should be in UploadParsedAsync(dto) workflow.
            if (dto == null 
                || dto.Data == null
                || dto.Data.Count == 0)
            {
                _logger.LogError("Results data is invalid");
                return BadRequest("Invalid data");
            }

            var result = await _uploadService.UploadParsedAsync(dto);

            return this.ToActionResult(result);
        }
    }
}
