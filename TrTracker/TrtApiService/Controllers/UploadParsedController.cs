﻿using Microsoft.AspNetCore.Mvc;
using TrtApiService.App.UploadParsedService;
using TrtShared.DTO;

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

        /// <summary>
        /// POST: api/UploadParsed
        /// POSTs all info about test run in to the DB
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Http result of the operation</returns>
        [HttpPost]
        public async Task<IActionResult> UploadParsedAsync([FromBody] TestRunDTO dto)
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
