using Microsoft.EntityFrameworkCore;
using TrtApiService.App.UploadParsedService;
using TrtApiService.Data;
using TrtApiService.Models;
using TrtApiService.Repositories;
using TrtShared.DTO;

namespace TrtApiService.Implementation.UploadParsedService
{
    public class EfCoreUploadParsedService : UploadParserServiceBase
    {
        private readonly TrtDbContext _context;
        private readonly ILogger _logger;
        private readonly IBranchRepository _branch;
        private readonly IResultRepository _result;
        private readonly ITestRepository _test;
        private readonly ITestrunRepository _testrun;

        public EfCoreUploadParsedService(TrtDbContext context, ILogger<EfCoreUploadParsedService> logger,
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

        public override async Task<bool> UploadParsedAsync(TestRunDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            int testRunId = 0;

            try
            {
                // Branch processing
                var branch = await _branch.GetOrCreateAsync(dto.Branch);

                // Test processing
                await _test.CreateAsync(await GetAddedTestsListFromDtoAsync(dto));

                // TestRun processing
                var testRun = await _testrun.CreateAsync(GetTestrunFromDto(dto, branch));
                testRunId = testRun.Id;

                // Results processing
                var testsDic = await GetCurTestsDicAsync(dto);
                await _result.CreateAsync(GetResultsListFromDto(dto, testRunId, testsDic));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process results uploading");
                await transaction.RollbackAsync();
                return false;
            }

            return true;
        }

        protected override async Task<IList<Test>> GetAddedTestsListFromDtoAsync(TestRunDTO dto)
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
            var newTests = dtoTestsList
                .Where(name => !dbTestsList.Contains(name))
                .Select(name => new Test { Name = name })
                .ToList();

            return newTests;
        }

        protected override async Task<IDictionary<string, int>> GetCurTestsDicAsync(TestRunDTO dto)
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

        protected override IList<Result> GetResultsListFromDto(TestRunDTO dto, int testRunId, IDictionary<string, int> testsDic)
        {
            var results = dto.Results
                .Select(r => new Result
                {
                    Outcome = r.Outcome,
                    ErrMsg = r.ErrMsg,
                    TestrunId = testRunId,
                    TestId = testsDic[r.TestName]
                }).ToList();

            return results;
        }

        protected override Testrun GetTestrunFromDto(TestRunDTO dto, Branch branch)
        {
            var testrun = new Testrun
            {
                Version = dto.Version,
                Date = dto.Date,
                BranchId = branch.Id,
            };

            return testrun;
        }
    }
}
