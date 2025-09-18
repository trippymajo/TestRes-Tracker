using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.Repositories
{
    public class TestrunRepository : Repository<Testrun>
    {
        public TestrunRepository(TrtDbContext context) : base(context) { }

        /// <summary>
        /// READ. Checks if the testrun already exists
        /// </summary>
        /// <param name="branchId">Branch id of the current testrun</param>
        /// <param name="version">Version of the product being tested</param>
        /// <returns>
        /// True - branch exists
        /// False - no such branch found
        /// </returns>
        public async Task<bool> IsExistsAsync(int branchId, string version)
        {
            return await _context.Testruns.AnyAsync(tr =>
                tr.BranchId == branchId &&
                tr.Version == version);
        }

        /// <summary>
        /// READ. Checks if the testrun with specified id already exists
        /// </summary>
        /// <param name="id">Tesrun id to check</param>
        /// True - testrun exists
        /// False - no testrun found
        /// </returns>
        public async Task<bool> IsExistsAsync(int id)
        {
            return await _context.Testruns.AnyAsync(tr => tr.Id == id);
        }

        /// <summary>
        /// READ. Gets testrun with Navigation Props loaded
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Testrun?> GetFullAsync(int id)
        {
            return await _context.Testruns
                    .Include(tr => tr.Branch)
                    .Include(tr => tr.Results)
                    .FirstOrDefaultAsync(tr => tr.Id == id);
        }
    }
}
