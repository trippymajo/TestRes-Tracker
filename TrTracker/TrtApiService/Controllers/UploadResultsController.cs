using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtShared.DTO;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadResultsController : ControllerBase
    {
        private readonly TrtDbContext _context;

        // Default Constructor
        public UploadResultsController(TrtDbContext context)
        {
            _context = context;
        }

        private async Task AddResults(TestRunDTO dto, Testrun testRun, IDictionary<string, Test> testsDic)
        {
            var results = dto.Results
                .Select(r => new Result
                {
                    Outcome = r.Outcome,
                    ErrMsg = r.ErrMsg,
                    TestrunId = testRun.Id,
                    TestId = testsDic[r.TestName].Id
                }).ToList();

            await _context.Results.AddRangeAsync(results);
        }

        /// <summary>
        /// Add new testRun into DB from testRun dto
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>TestRun reference in DB</returns>
        private async Task<Testrun> AddTestRun(TestRunDTO dto, Branch branch)
        {
            var testrun = new Testrun
            {
                Version = dto.Version,
                Date = dto.Date,
                BranchId = branch.Id,
            };

            await _context.Testruns.AddAsync(testrun);

            return testrun;
        }

        /// <summary>
        /// Add new branch into DB from testRun dto
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Branch reference in db</returns>
        private async Task<Branch> AddNewBranch(TestRunDTO dto)
        {
            var branch = await _context.Branches
                    .FirstOrDefaultAsync(b => b.Name == dto.Branch);

            // If branch name was not found
            if (branch == null)
            {
                branch = new Branch { Name = dto.Branch };
                await _context.Branches.AddAsync(branch);
            }

            return branch;
        }

        /// <summary>
        /// Add new tests into DB from testRun dto
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        private async Task AddNewTests(TestRunDTO dto)
        {
            // Dto list of testNames
            var dtoTestsList = dto.Results
                .Select(r => r.TestName)
                .Distinct()
                .ToList();

            // DB testNames which matches dto names list
            var dbTestsList = await _context.Tests
                .Where(t => dtoTestsList.Contains(t.Name))
                .Select(t => t.Name)
                .ToListAsync();

            var newTests = dto.Results
                .Where(r => !dbTestsList.Contains(r.TestName))
                .Select(t => new Test
                {
                    Name = t.TestName
                    // May be description will be needed to paste here 
                    // Description = t.Description
                })
                .ToList();

            await _context.AddRangeAsync(newTests);
        }

        /// <summary>
        /// Gets curent dto's dictionary from DB
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Dictionary of tests in Db</returns>
        private async Task<IDictionary<string, Test>> GetCurTestsDic(TestRunDTO dto)
        {
            // Dto list of testNames
            var dtoTestsList = dto.Results
                .Select(r => r.TestName)
                .Distinct()
                .ToList();

            return await _context.Tests
                .Where(t => dtoTestsList.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t);
        }

        /// <summary>
        /// POSTs all info about test run in to the DB
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Http result of the operation</returns>
        [HttpPost]
        public async Task<IActionResult> UploadTestRun([FromBody] TestRunDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Branch)
                || string.IsNullOrWhiteSpace(dto.Version))
            {
                return BadRequest("Invalid data");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            int testRunId = 0;

            try
            {
                // Branch processing
                var branch = await AddNewBranch(dto);

                // Test processing
                await AddNewTests(dto);

                // TestRun processing
                var testRun = await AddTestRun(dto, branch);
                testRunId = testRun.Id;

                // Results processing
                var testDic = await GetCurTestsDic(dto);
                await AddResults(dto, testRun, testDic);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Failed to process testRun upload");
            }

            return Ok(testRunId);
        }
    }
}
