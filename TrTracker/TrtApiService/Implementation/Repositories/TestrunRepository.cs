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
        /// <param name="date">Date when TestRun has been held</param>
        /// <returns>
        /// True - branch exists
        /// False - no such branch found
        /// </returns>
        public async Task<bool> IsExistsAsync(int branchId, string version, DateTime date)
        {
            var goodDate = date.Kind == DateTimeKind.Utc
                ? date
                : date.ToUniversalTime();

            return await _context.Testruns.AnyAsync(tr =>
                tr.BranchId == branchId &&
                tr.Version == version &&
                tr.Date == goodDate);
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

        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="testrun">Testrun entity to update</param>
        /// <param name="version">Version of product where testrun being held</param>
        /// <param name="date">Date when testrun was done</param>
        /// <param name="branchId">Corresponding branch of the testrun</param>
        public void Update(Testrun testrun, string version, DateTime date, int branchId)
        {
            testrun.Version = version;
            testrun.BranchId = branchId;
            testrun.Date = date.Kind == DateTimeKind.Utc
                ? date
                : date.ToUniversalTime();
        }
    }
}
