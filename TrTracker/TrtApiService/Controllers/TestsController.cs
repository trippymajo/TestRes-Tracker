using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrtApiService.App.CrudServices;
using TrtApiService.DTOs;

using TrtShared.RetValExtensions;

namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ICrudTestService _crudTest;

        public TestsController(ICrudTestService testService)
        {
            _crudTest = testService;
        }

        // GET: api/Tests
        [HttpGet]
        public async Task<IActionResult> GetTests()
        {
            var result = await _crudTest.GetTestsAsync();
            return this.ToActionResult(result);
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTest(int id)
        {
            var result = await _crudTest.GetTestAsync(id);
            return this.ToActionResult(result);
        }

        // POST: api/Tests
        [HttpPost]
        [Authorize(Policy = "CanManageTests")]
        public async Task<IActionResult> PostTest([FromBody] CUTestDTO dto)
        {
            var result = await _crudTest.CreateTestAsync(dto);
            if (!result.Success)
                return this.ToActionResult(result);

            return CreatedAtAction(nameof(GetTest), new { id = result.Value }, new { id = result.Value });
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageTests")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] CUTestDTO dto)
        {
            var result = await _crudTest.UpdateTestAsync(id, dto);
            return this.ToActionResult(result);
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageTests")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var result = await _crudTest.DeleteTestAsync(id);
            return this.ToActionResult(result);
        }
    }
}
