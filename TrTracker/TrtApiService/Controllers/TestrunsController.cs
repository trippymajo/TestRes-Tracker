using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;

// WIP
namespace TrtApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestrunsController : ControllerBase
    {
        private readonly TrtDbContext _context;

        public TestrunsController(TrtDbContext context)
        {
            _context = context;
        }

        // GET: api/Testruns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testrun>>> GetTestruns()
        {
            return await _context.Testruns.ToListAsync();
        }

        // GET: api/Testruns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Testrun>> GetTestrun(int id)
        {
            var testrun = await _context.Testruns.FindAsync(id);

            if (testrun == null)
            {
                return NotFound();
            }

            return testrun;
        }

        // PUT: api/Testruns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestrun(int id, Testrun testrun)
        {
            if (id != testrun.Id)
            {
                return BadRequest();
            }

            _context.Entry(testrun).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestrunExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Testruns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Testrun>> PostTestrun(Testrun testrun)
        {
            _context.Testruns.Add(testrun);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestrun", new { id = testrun.Id }, testrun);
        }

        // DELETE: api/Testruns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestrun(int id)
        {
            var testrun = await _context.Testruns.FindAsync(id);
            if (testrun == null)
            {
                return NotFound();
            }

            _context.Testruns.Remove(testrun);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestrunExists(int id)
        {
            return _context.Testruns.Any(e => e.Id == id);
        }
    }
}
