using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.Repositories
{
    public class ResultRepository : Repository<Result>
    {
        public ResultRepository(TrtDbContext context) : base(context) { }

        /// <summary>
        /// READ. Checks if the result for specified test and testrun exists
        /// </summary>
        /// <param name="testId">test id of corresponding result</param>
        /// <param name="testrunId">testrun id of corresponding result</param>
        /// <returns>
        /// True - result exists
        /// False - no result found
        /// </returns>
        public async Task<bool> IsExistsAsync(int testId, int testrunId)
        {
            return await _context.Results.AnyAsync(r =>
                r.TestId == testId &&
                r.TestrunId == testrunId);
        }

        /// <summary>
        /// READ. Checks if the result with specified id already exists
        /// </summary>
        /// <param name="id">Result id to check</param>
        /// <returns>
        /// True - result exists
        /// False - no result found
        /// </returns>
        public async Task<bool> IsExistsAsync(int id)
        {
            return await _context.Tests.AnyAsync(t => t.Id == id);
        }

        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="result">Result entity to update</param>
        /// <param name="outcome">Result of the specified test in testrun</param>
        /// <param name="errMsg">Error Message which was provided</param>
        public void Update(Result result, string outcome, string? errMsg = null)
        {
            result.Outcome = outcome;
            result.ErrMsg = errMsg;
        }
    }
}
