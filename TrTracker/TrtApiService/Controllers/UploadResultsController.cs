using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtApiService.Repositories;
using TrtShared.DTO;

namespace TrtApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadResultsController : ControllerBase
    {
        private readonly TrtDbContext _context;
        private readonly ILogger _logger;
        private readonly IBranchRepository _branch;
        private readonly IResultRepository _result;
        private readonly ITestRepository _test;
        private readonly ITestrunRepository _testrun;

        // Default Constructor
        public UploadResultsController(TrtDbContext context, ILogger<UploadResultsController> logger,
            IBranchRepository branch, IResultRepository result,
            ITestRepository test, ITestrunRepository testrun)
        {
            _context = context;
            _logger = logger;
            _branch = branch;
            _result = result;
            _test = test;
            _testrun = testrun;
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
        /// Gets list of new Test names from TestRun DTO
        /// </summary>
        /// <param name="dto">DTO to get information from</param>
        /// <returns>List of new tests from Dto</returns>
        private async Task<IList<Test>> GetNewTestNamesListFromDto(TestRunDTO dto)
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

            // Only just new tests
            var newTests = dbTestsList
                .Where(name => !dbTestsList.Contains(name))
                .Select(name => new Test { Name = name })
                .ToList();

            return newTests;
        }

        /// <summary>
        /// Gets current TestRun model item from TestRunDto
        /// </summary>
        /// <param name="dto">TestRunDTO to make an item from</param>
        /// <param name="branch">Branch of current testrun</param>
        /// <returns>TestRun Model</returns>
        private Testrun GetTestrunFromDto(TestRunDTO dto, Branch branch)
        {
            var testrun = new Testrun
            {
                Version = dto.Version,
                Date = dto.Date,
                BranchId = branch.Id,
            };

            return testrun;
        }

        /// <summary>
        /// Gets curent dto's dictionary from DB
        /// </summary>
        /// <param name="dto">TestRun </param>
        /// <returns>Dictionary of tests in Db</returns>
        private async Task<IDictionary<string, int>> GetCurTestsDic(TestRunDTO dto)
        {
            // Dto's list of testNames
            var testNamesList = dto.Results
                .Select(r => r.TestName)
                .Distinct()
                .ToList();

            return await _context.Tests
                .Where(t => testNamesList.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t.Id);
        }

        private IList<Result> GetResultsListFromDto(TestRunDTO dto, Testrun testRun, IDictionary<string, int> testsDic)
        {
            var results = dto.Results
                .Select(r => new Result
                {
                    Outcome = r.Outcome,
                    ErrMsg = r.ErrMsg,
                    TestrunId = testRun.Id,
                    TestId = testsDic[r.TestName]
                }).ToList();

            return results;
        }

        /// <summary>
        /// POST: api/UploadResults
        /// POSTs all info about test run in to the DB
        /// </summary>
        /// <param name="dto">Data to put in to DB</param>
        /// <returns>Http result of the operation</returns>
        [HttpPost]
        public async Task<IActionResult> UploadResultsAsync([FromBody] TestRunDTO dto)
        {
            if (dto == null 
                || dto.Results.Any() == false 
                || string.IsNullOrWhiteSpace(dto.Branch)
                || string.IsNullOrWhiteSpace(dto.Version))
            {
                _logger.LogError("Results data is invalid");
                return BadRequest("Invalid data");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            int testRunId = 0;

            try
            {
                // Branch processing
                var branch = await _branch.GetOrCreateAsync(dto.Branch);

                // Test processing
                await _test.CreateAsync(await GetNewTestNamesListFromDto(dto));

                // TestRun processing
                var testRun = await _testrun.CreateAsync(GetTestrunFromDto(dto, branch));
                testRunId = testRun.Id;

                // Results processing
                var testsDic = await GetCurTestsDic(dto);
                await _result.CreateAsync(GetResultsListFromDto(dto, testRun, testsDic));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process results uploading");
                await transaction.RollbackAsync();
                return StatusCode(500, "Failed to process testRun upload");
            }

            return Ok(testRunId);
        }
    }
}
