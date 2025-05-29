using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly TrtDbContext _context;

        public BranchesController(TrtDbContext context)
        {
            _context = context;
        }

        // GET: api/Branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Branch>>> GetBranches()
        {
            return await _context.Branches.ToListAsync();
        }

        // GET: api/Branches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetBranches(int id)
        {
            var branch = await _context.Branches.FindAsync(id);

            if (branch == null)
            {
                return NotFound();
            }

            return branch;
        }

        // PUT: api/Branches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Renames selected branch
        /// </summary>
        /// <param name="id">Id of branch in DB to rename</param>
        /// <param name="strNewName">New name for the selected branch</param>
        /// <returns>Http result</returns>
        [Authorize(Policy = "CanManageBranches")]
        [HttpPut("{id}")]
        public async Task<IActionResult> RenameBranch(int id, [FromBody] string strNewName)
        {
            if (string.IsNullOrWhiteSpace(strNewName))
                return BadRequest("New branch name is null");

            try
            {
                var branch = await _context.Branches.FindAsync(id);
                if (branch == null)
                {
                    return NotFound("Branch not found");
                }

                var branchExists = await _context.Branches.AnyAsync(b => b.Name == strNewName);
                if (branchExists)
                {
                    return BadRequest("Branch with such name already exists");
                }


                branch.Name = strNewName;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to update the branch. Database error.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to rename the branch");
            }

            return NoContent();
        }

        // POST: api/Branches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Branch>> PostBranch(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBranch", new { id = branch.Id }, branch);
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}
