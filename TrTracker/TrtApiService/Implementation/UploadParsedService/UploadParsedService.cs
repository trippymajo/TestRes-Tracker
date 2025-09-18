using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using TrtApiService.App.UploadParsedService;
using TrtApiService.Data;
using TrtApiService.Implementation.Repositories;
using TrtApiService.Models;
using TrtShared.DTO;
using TrtShared.Envelope;
using TrtShared.RetValType;

// static usage
using static TrtShared.Envelope.UniEnvelopeHelpers;

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

        public async Task<RetVal> UploadParsedAsync(UniEnvelope envelope)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // === Branch ===
                var branchName = GetString(envelope.Data, UniEnvelopeSchema.Branch);
                if (string.IsNullOrWhiteSpace(branchName))
                    return RetVal.Fail(ErrorType.BadRequest, "Branch is required");
                var branch = await _branch.GetOrCreateAsync(branchName);

                // === Test ===
                await _test.CreateAsync(await GetAddedTestsListFromDtoAsync(envelope));
                await _context.SaveChangesAsync(); // Save changes to get ids of branches and tests

                // === Idempotency ===
                var idemKey = GetString(envelope.Data, UniEnvelopeSchema.IdempotencyKey);
                if (!string.IsNullOrWhiteSpace(idemKey))
                {
                    var exists = await _context.Testruns.AnyAsync(tr => tr.IdempotencyKey == idemKey);
                    if (exists)
                    {
                        await transaction.CommitAsync();
                        _logger.LogInformation("Idempotent replay ignored (key={Key})", idemKey);
                        return RetVal.Ok();
                    }
                }

                // === Testrun ===
                var testRun = await _testrun.CreateAsync(GetTestrunFromDto(envelope, branch));
                await _context.SaveChangesAsync(); // Save changes to get ids of testrun

                // === Results ===
                var resultsArr = GetList(envelope.Data, UniEnvelopeSchema.Results) ?? new();
                if (resultsArr.Count > 0)
                {
                    var testsDic = await GetCurTestsDicAsync(envelope); // name -> id
                    var batch = GetResultsListFromDto(envelope, testRun.Id, testsDic);
                    if (batch.Count > 0)
                    {
                        await _result.CreateAsync(batch);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Successfully pushed to DB: Branch: {Branch}, TestrunId: {TestRunId}",
                    branch.Name, testRun.Id);

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = "Failed to process results uploading";
                _logger.LogError(ex, errMsg);
                await transaction.RollbackAsync();
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        private async Task<IList<Test>> GetAddedTestsListFromDtoAsync(UniEnvelope envelope)
        {
            var testsArr = GetList(envelope.Data, UniEnvelopeSchema.Tests) ?? new();

            // UniEnvelope Tests' names
            var uniTestsList = testsArr
                .Select(t => GetString(t, UniEnvelopeSchema.TestInfo.Name))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            // DB testNames which matches uni names list
            var dbTestsList = await _context.Tests
                .Where(t => uniTestsList.Contains(t.Name))
                .Select(t => t.Name)
                .ToListAsync();

            // Only just new tests
            var newTests = uniTestsList
                .Where(name => !dbTestsList.Contains(name!))
                .Select(name => new Test 
                { 
                    Name = name!,

                    ClassName = GetString(
                        testsArr.First(tt => GetString(tt, UniEnvelopeSchema.TestInfo.Name) == name!),
                        UniEnvelopeSchema.TestInfo.ClassName),

                    Description = GetString(
                        testsArr.First(tt => GetString(tt, UniEnvelopeSchema.TestInfo.Name) == name!),
                        UniEnvelopeSchema.TestInfo.Desctiption)
                })
                .ToList();

            return newTests;
        }

        /// <summary>
        /// Gets current tests from DB
        /// </summary>
        private async Task<IDictionary<string, int>> GetCurTestsDicAsync(UniEnvelope envelope)
        {
            var testsArr = GetList(envelope.Data, UniEnvelopeSchema.Tests) ?? new();

            // Dto's list of testNames
            var testNamesList = testsArr
                .Select(t => GetString(t, UniEnvelopeSchema.TestInfo.Name))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            return await _context.Tests
                .Where(t => testNamesList.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name, t => t.Id);
        }

        private IList<Result> GetResultsListFromDto(UniEnvelope envelope, int testRunId, IDictionary<string, int> testsDic)
        {
            var testsArr = GetList(envelope.Data, UniEnvelopeSchema.Tests) ?? new();
            var resultsArr = GetList(envelope.Data, UniEnvelopeSchema.Results) ?? new();

            var count = Math.Min(testsArr.Count, resultsArr.Count);
            var results = new List<Result>(count);

            for (int i = 0; i < count; i++)
            {
                var t = testsArr[i];
                var r = resultsArr[i];

                var testName = GetString(t, UniEnvelopeSchema.TestInfo.Name);
                if (string.IsNullOrWhiteSpace(testName) || !testsDic.TryGetValue(testName, out var testId))
                {
                    // Bad binding
                    _logger.LogWarning("Result index {Index} skipped: test not found (Name={Name})", i, testName);
                    continue;
                }

                results.Add(new Result
                {
                    TestrunId = testRunId,
                    TestId = testId,

                    Outcome = GetString(r, UniEnvelopeSchema.ResultInfo.Outcome) ?? string.Empty,
                    StartedAt = GetDate(r, UniEnvelopeSchema.ResultInfo.StartedAt),
                    FinishedAt = GetDate(r, UniEnvelopeSchema.ResultInfo.FinishedAt),
                    DurationMs = GetLong(r, UniEnvelopeSchema.ResultInfo.DurationMs),
                    ErrType = GetString(r, UniEnvelopeSchema.ResultInfo.ErrType),
                    ErrMsg = GetString(r, UniEnvelopeSchema.ResultInfo.ErrMsg),
                    ErrStack = GetString(r, UniEnvelopeSchema.ResultInfo.ErrStack),
                    StdOut = GetString(r, UniEnvelopeSchema.ResultInfo.StdOut),
                    StdErr = GetString(r, UniEnvelopeSchema.ResultInfo.StdErr),
                });
            }

            return results;
        }

        private Testrun GetTestrunFromDto(UniEnvelope envelope, Branch branch)
        {
            // Basic Testrun info
            var testrun = new Testrun
            {
                BranchId = branch.Id,
                Version = GetString(envelope.Data, UniEnvelopeSchema.Version) ?? string.Empty,
                StartedAt = GetDate(envelope.Data, UniEnvelopeSchema.StartedAt),
                FinishedAt = GetDate(envelope.Data, UniEnvelopeSchema.FinishedAt),
                DurationMs = GetLong(envelope.Data, UniEnvelopeSchema.DurationMs),
                EnvironmentJson = GetString(envelope.Data, UniEnvelopeSchema.EnvironmentJson),
                IdempotencyKey = GetString(envelope.Data, UniEnvelopeSchema.IdempotencyKey)
            };

            // Aggregates
            var aggs = GetDict(envelope.Data, UniEnvelopeSchema.Agregates);
            if (aggs != null)
            {
                testrun.Total = GetInt(aggs, UniEnvelopeSchema.TestrunAgregates.Total);
                testrun.Passed = GetInt(aggs, UniEnvelopeSchema.TestrunAgregates.Passed);
                testrun.Failed = GetInt(aggs, UniEnvelopeSchema.TestrunAgregates.Failed);
                testrun.Skipped = GetInt(aggs, UniEnvelopeSchema.TestrunAgregates.Skipped);
                testrun.Errors = GetInt(aggs, UniEnvelopeSchema.TestrunAgregates.Errors);
            }

            return testrun;
        }
    }
}
