using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TrtApiService.Models;
using TrtApiService.App.CrudServices;
using TrtShared.RetValExtensions;

namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly ICrudBranchService _crudBranch;
        private readonly ILogger<BranchesController> _logger;

        public BranchesController(ICrudBranchService crudBranch, ILogger<BranchesController> logger)
        {
            _crudBranch = crudBranch;
            _logger = logger;
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
        [Authorize(Policy = "CanManageBranches")]
        [HttpPut("{id}")]
        public async Task<IActionResult> RenameBranch(int id, [FromBody] string strNewName)
        {
            var result = await _crudBranch.RenameBranchAsync(id, strNewName);
            return this.ToActionResult(result);
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "CanManageBranches")]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            var result = await _crudBranch.CreateBranchAsync(branch);
            return this.ToActionResult(result);
        }
    }
}
