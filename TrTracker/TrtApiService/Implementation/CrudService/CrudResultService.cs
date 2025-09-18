using Humanizer;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
using TrtApiService.Implementation.Repositories;
using TrtApiService.Models;
using TrtShared.RetValType;

namespace TrtApiService.Implementation.CrudService
{
    public class CrudResultService : ICrudResultService
    {
        private readonly TrtDbContext _context;
        private readonly ResultRepository _result;
        private readonly TestRepository _test;
        private readonly TestrunRepository _testrun;
        private readonly ILogger<CrudResultService> _logger;

        public CrudResultService(TrtDbContext context, ResultRepository result,
            TestRepository test, TestrunRepository testrun,
            ILogger<CrudResultService> logger)
        {
            _context = context;
            _result = result;
            _test = test;
            _testrun = testrun;
            _logger = logger;
        }

        public async Task<RetVal<int>> CreateResultAsync(CreateResultDTO resultDto)
        {
            // === Validation ===
            if (string.IsNullOrWhiteSpace(resultDto.Outcome))
            {
                var errMsg = "Test Outcome is required for result";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            if (resultDto.TestrunId <= 0)
            {
                var errMsg = "Testrun Id must be positive";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            if (resultDto.TestId <= 0)
            {
                var errMsg = "Test Id must be positive";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            if (!await _test.IsExistsAsync(resultDto.TestId)
                || !await _testrun.IsExistsAsync(resultDto.TestrunId))
            {
                var errMsg = $"Result for test id {resultDto.TestId} and testun id {resultDto.TestrunId} was not found";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.NotFound, errMsg);
            }

            // Dont allow dublicates (TestId,TestrunId)
            if (await _result.IsExistsAsync(resultDto.TestId, resultDto.TestrunId))
            {
                var errMsg = $"Result for Test {resultDto.TestId} in Testrun {resultDto.TestrunId} already exists";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.Conflict, errMsg);
            }

            try
            {
                static DateTimeOffset? ToUtc(DateTimeOffset? t) => t?.ToUniversalTime();

                var startedUtc = ToUtc(resultDto.StartedAt);
                var finishedUtc = ToUtc(resultDto.FinishedAt);

                long? durationMs = null;
                if (startedUtc.HasValue && finishedUtc.HasValue)
                    durationMs = (long)(finishedUtc.Value - startedUtc.Value).TotalMilliseconds;

                var result = new Result
                {
                    Outcome = resultDto.Outcome,
                    StartedAt = startedUtc,
                    FinishedAt = finishedUtc,
                    DurationMs = durationMs,

                    ErrType = resultDto.ErrType,
                    ErrMsg = resultDto.ErrMsg,
                    ErrStack = resultDto.ErrStack,
                    StdOut = resultDto.StdOut,
                    StdErr = resultDto.StdErr,

                    TestId = resultDto.TestId,
                    TestrunId = resultDto.TestrunId
                };

                await _result.CreateAsync(result);
                await _context.SaveChangesAsync();

                return RetVal<int>.Ok(result.Id);
            }
            catch (Exception ex)
            {
                var errMsg = "Creating new result failed";
                _logger.LogError(ex, errMsg);
                return RetVal<int>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> DeleteResultAsync(int id)
        {
            try
            {
                var result = await _result.FindByIdAsync(id);
                if (result == null)
                {
                    var errMsg = $"Result with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _result.Remove(result);
                await _context.SaveChangesAsync();

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Deleting result with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<Result>> GetResultAsync(int id)
        {
            try
            {
                var result = await _context.Results.FindAsync(id);
                if (result == null)
                {
                    var errMsg = $"Result with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal<Result>.Fail(ErrorType.NotFound, errMsg);
                }

                return RetVal<Result>.Ok(result);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting result with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal<Result>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<IEnumerable<Result>>> GetResultsAsync()
        {
            try
            {
                var results = await _result.GetAsync();

                return RetVal<IEnumerable<Result>>.Ok(results);
            }
            catch (Exception ex)
            {
                var errMsg = "Getting all results failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Result>>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> UpdateResultAsync(int id, UpdateResultDTO resultDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(resultDto.Outcome))
            {
                var errMsg = "Test Outcome is required for result";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.BadRequest, errMsg);
            }

            try
            {
                var result = await _result.FindByIdAsync(id);
                if (result == null)
                {
                    var errMsg = $"Result with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                static DateTimeOffset? ToUtc(DateTimeOffset? t) => t?.ToUniversalTime();

                result.Outcome = resultDto.Outcome;
                if (resultDto.ErrMsg != null) result.ErrMsg = resultDto.ErrMsg;
                if (resultDto.ErrType != null) result.ErrType = resultDto.ErrType;
                if (resultDto.ErrStack != null) result.ErrStack = resultDto.ErrStack;
                if (resultDto.StdOut != null) result.StdOut = resultDto.StdOut;
                if (resultDto.StdErr != null) result.StdErr = resultDto.StdErr;

                if (resultDto.StartedAt.HasValue)
                    result.StartedAt = ToUtc(resultDto.StartedAt);

                if (resultDto.FinishedAt.HasValue)
                    result.FinishedAt = ToUtc(resultDto.FinishedAt);

                result.DurationMs = (result.StartedAt.HasValue && result.FinishedAt.HasValue)
                    ? (long?)(result.FinishedAt.Value - result.StartedAt.Value).TotalMilliseconds
                    : null;

                _result.Update(result);
                await _context.SaveChangesAsync();
                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Updating result with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }
    }
}