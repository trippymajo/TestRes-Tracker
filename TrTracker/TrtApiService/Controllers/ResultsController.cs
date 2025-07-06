using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrtApiService.App.CrudServices;
using TrtApiService.DTOs;

using TrtShared.RetValExtensions;

namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ICrudResultService _crudResult;

        public ResultsController(ICrudResultService crudResult)
        {
            _crudResult = crudResult;
        }

        // GET: api/Results
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var result = await _crudResult.GetResultsAsync();
            return this.ToActionResult(result);
        }

        // GET: api/Results/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _crudResult.GetResultAsync(id);
            return this.ToActionResult(result);
        }

        // POST: api/Results
        [HttpPost]
        [Authorize(Policy = "CanManageResults")]
        public async Task<IActionResult> PostResult([FromBody] CreateResultDTO dto)
        {
            var result = await _crudResult.CreateResultAsync(dto);
            if (!result.Success)
                return this.ToActionResult(result);

            return CreatedAtAction(nameof(GetResult), new { id = result.Value }, new { id = result.Value });
        }

        // PUT: api/Results/5
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageResults")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] UpdateResultDTO dto)
        {
            var result = await _crudResult.UpdateResultAsync(id, dto);
            return this.ToActionResult(result);
        }

        // DELETE: api/Results/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageResults")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _crudResult.DeleteResultAsync(id);
            return this.ToActionResult(result);
        }
    }
}
