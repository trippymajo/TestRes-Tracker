using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.DTOs;
using TrtApiService.Models;
using TrtShared.RetValExtensions;

// WIP
namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestrunsController : ControllerBase
    {
        private readonly ICrudTestrunService _crudTestrun;

        public TestrunsController(ICrudTestrunService crudTestrun)
        {
            _crudTestrun = crudTestrun;
        }

        // GET: api/Testruns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testrun>>> GetTestruns()
        {
            var result = await _crudTestrun.GetTestrunsAsync();
            return this.ToActionResult(result);
        }

        // GET: api/Testruns/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestrun(int id)
        {
            var result = await _crudTestrun.GetTestrunAsync(id);
            return this.ToActionResult(result);
        }

        // PUT: api/Testruns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageTestruns")]
        public async Task<IActionResult> UpdateTestrun(int id, [FromBody] CUTestrunDTO testrunDto)
        {
            var result = await _crudTestrun.UpdateTestrunAsync(id, testrunDto);
            return this.ToActionResult(result);
        }

        // POST: api/Testruns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "CanManageTestruns")]
        public async Task<IActionResult> PostTestrun([FromBody] CUTestrunDTO testrunDto)
        {
            var result = await _crudTestrun.CreateTestrunAsync(testrunDto);
            return this.ToActionResult(result);
        }

        // DELETE: api/Testruns/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageTestruns")]
        public async Task<IActionResult> DeleteTestrun(int id)
        {
            var result = await _crudTestrun.DeleteTestrunAsync(id);
            return this.ToActionResult(result);
        }
    }
}
