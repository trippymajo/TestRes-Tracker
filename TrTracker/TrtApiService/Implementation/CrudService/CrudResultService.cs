using TrtApiService.Implementation.Repositories;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
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
            // Validation
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

            try
            {
                var result = new Result
                {
                    Outcome = resultDto.Outcome,
                    ErrMsg = resultDto.ErrMsg,
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

                _result.Update(result, resultDto.Outcome, resultDto.ErrMsg);
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