using Microsoft.EntityFrameworkCore;

using TrtApiService.Data;
using TrtApiService.Models;
using TrtApiService.App.UploadParsedService;
using TrtApiService.Implementation.Repositories;

using TrtShared.RetValType;
using TrtShared.DTO;

namespace TrtApiService.Implementation.UploadParsedService
{
    public class UploadParsedService : IUploadParsedService
    {
        private readonly TrtDbContext _context;
        private readonly ILogger _logger;
        private readonly BranchRepository _branch;
        private readonly ResultRepository _result;
        private readonly TestRepository _test;
        private readonly TestrunRepository _testrun;

        public UploadParsedService(TrtDbContext context, ILogger<UploadParsedService> logger,
            BranchRepository branch, ResultRepository result,
            TestRepository test, TestrunRepository testrun)
        {
            _context = context;
            _logger = logger;
            _branch = branch;
            _result = result;
            _test = test;
            _testrun = testrun;
        }

        public async Task<RetVal> UploadParsedAsync(TestRunDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Branch processing
                var branch = await _branch.GetOrCreateAsync(dto.Branch);

                // Test processing
                await _test.CreateAsync(await GetAddedTestsListFromDtoAsync(dto));

                // Save changes to get ids of branches and tests
                await _context.SaveChangesAsync();

                // TestRun processing
                var testRun = await _testrun.CreateAsync(GetTestrunFromDto(dto, branch));
                await _context.SaveChangesAsync();

                // Results processing
                var testsDic = await GetCurTestsDicAsync(dto);
                await _result.CreateAsync(GetResultsListFromDto(dto, testRun.Id, testsDic));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Successfully pushed to DB: Branch: {Branch}, TestrunId: {TestRunId}",
                    branch.Name, testRun.Id);
            }
            catch (Exception ex)
            {
                string errMsg = "Failed to process results uploading";
                _logger.LogError(ex, errMsg);
                await transaction.RollbackAsync();
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal.Ok();
        }

        private async Task<IList<Test>> GetAddedTestsListFromDtoAsync(TestRunDTO dto)
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

        private async Task<IDictionary<string, int>> GetCurTestsDicAsync(TestRunDTO dto)
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

        private IList<Result> GetResultsListFromDto(TestRunDTO dto, int testRunId, IDictionary<string, int> testsDic)
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

        private Testrun GetTestrunFromDto(TestRunDTO dto, Branch branch)
        {
            var testrun = new Testrun
            {
                Version = dto.Version,
                Date = dto.Date.Kind == DateTimeKind.Utc
                    ? dto.Date
                    : dto.Date.ToUniversalTime(),
                BranchId = branch.Id,
            };

            return testrun;
        }
    }
}
