using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrtApiService.Models;
using TrtApiService.App.CrudServices;
using TrtApiService.DTOs;

using TrtShared.RetValExtensions;

namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly ICrudBranchService _crudBranch;

        public BranchesController(ICrudBranchService crudBranch)
        {
            _crudBranch = crudBranch;
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            var result = await _crudBranch.GetBranchesAsync();
            return this.ToActionResult(result);
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranch(int id)
        {
            var result = await _crudBranch.GetBranchAsync(id);
            return this.ToActionResult(result);
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageBranches")]
        public async Task<IActionResult> RenameBranch(int id, [FromBody] CUBranchDTO strNewName)
        {
            var result = await _crudBranch.UpdateBranchAsync(id, strNewName);
            return this.ToActionResult(result);
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "CanManageBranches")]
        public async Task<ActionResult<Branch>> CreateBranch([FromBody] CUBranchDTO branch)
        {
            var result = await _crudBranch.CreateBranchAsync(branch);
            return this.ToActionResult(result);
        }

        // DELETE: api/Branches/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageBranches")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _crudBranch.DeleteBranchAsync(id);
            return this.ToActionResult(result);
        }
    }
}
