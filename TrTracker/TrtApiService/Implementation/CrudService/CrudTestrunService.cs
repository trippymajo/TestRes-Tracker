using Humanizer;
using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
using TrtApiService.Implementation.Repositories;
using TrtApiService.Models;
using TrtShared.RetValType;

namespace TrtApiService.Implementation.CrudService
{
    public class CrudTestrunService : ICrudTestrunService
    {
        private readonly TrtDbContext _context;
        private readonly TestrunRepository _testrun;
        private readonly BranchRepository _branch;
        private readonly ILogger<CrudTestrunService> _logger;

        public CrudTestrunService(TrtDbContext context, TestrunRepository testrun, BranchRepository branch, ILogger<CrudTestrunService> logger)
        {
            _context = context;
            _testrun = testrun;
            _branch = branch;
            _logger = logger;
        }

        public async Task<RetVal<int>> CreateTestrunAsync(CUTestrunDTO testrunDto)
        {
            // === Validation ===
            if (!IsValidDto(testrunDto))
            {
                var errMsg = "Testrun data provided is not valid";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            if (!await _branch.IsExistsAsync(testrunDto.BranchId))
                return RetVal<int>.Fail(ErrorType.NotFound, $"Branch with id {testrunDto.BranchId} was not found.");

            static DateTimeOffset? ToUtc(DateTimeOffset? t) => t?.ToUniversalTime();
            var startedUtc = ToUtc(testrunDto.StartedAt);
            var finishedUtc = ToUtc(testrunDto.FinishedAt);
            long? durationMs = (startedUtc.HasValue && finishedUtc.HasValue)
                ? (long?)(finishedUtc.Value - startedUtc.Value).TotalMilliseconds
                : null;

            var exists = await _testrun.IsExistsAsync(testrunDto.BranchId, testrunDto.Version);

            if (exists)
            {
                var errMsg = "Such testrun already is in the DB";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.Conflict, errMsg);
            }

            if (!string.IsNullOrWhiteSpace(testrunDto.IdempotencyKey))
            {
                var keyExists = await _context.Testruns
                    .AnyAsync(tr => tr.IdempotencyKey == testrunDto.IdempotencyKey);

                if (keyExists)
                    return RetVal<int>.Fail(ErrorType.Conflict, "Testrun with the same idempotency key already exists");
            }

            var testrun = new Testrun
            {
                Version = testrunDto.Version,
                BranchId = testrunDto.BranchId,
                StartedAt = startedUtc,
                FinishedAt = finishedUtc,
                DurationMs = durationMs,
                EnvironmentJson = testrunDto.EnvironmentJson,
                IdempotencyKey = testrunDto.IdempotencyKey
            };

            try
            {
                await _testrun.CreateAsync(testrun);
                await _context.SaveChangesAsync();
                return RetVal<int>.Ok(testrun.Id);
            }
            catch (Exception ex)
            {
                var errMsg = "Failed to create testrun.";
                _logger.LogError(ex, errMsg);
                return RetVal<int>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> DeleteTestrunAsync(int id)
        {
            if (id <= 0)
                return RetVal.Fail(ErrorType.BadRequest, "Id must be positive.");

            try
            {
                var testrun = await _testrun.FindByIdAsync(id);
                if (testrun == null)
                {
                    var errMsg = $"Testrun with id {id} was not found.";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _testrun.Remove(testrun);
                await _context.SaveChangesAsync();

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Failed to delete testrun with id {id}.";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<Testrun>> GetTestrunAsync(int id)
        {
            if (id <= 0)
                return RetVal<Testrun>.Fail(ErrorType.BadRequest, "Id must be positive.");

            try
            {
                var testrun = await _testrun.FindByIdAsync(id);

                if (testrun == null)
                {
                    var errMsg = $"Testrun with id {id} was not found.";
                    _logger.LogWarning(errMsg);
                    return RetVal<Testrun>.Fail(ErrorType.NotFound, errMsg);
                }

                return RetVal<Testrun>.Ok(testrun);
            }
            catch (Exception ex)
            {
                var errMsg = $"Failed to get testrun with id {id}.";
                _logger.LogError(ex, errMsg);
                return RetVal<Testrun>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<IEnumerable<Testrun>>> GetTestrunsAsync()
        {
            try
            {
                var testrunList = await _testrun.GetAsync();

                return RetVal<IEnumerable<Testrun>>.Ok(testrunList);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting all the testruns failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Testrun>>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> UpdateTestrunAsync(int id, CUTestrunDTO testrunDto)
        {
            // === Validation ===
            if (!IsValidDto(testrunDto))
            {
                var errMsg = "Testrun data provided is not valid";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.BadRequest, errMsg);
            }

            if (!await _branch.IsExistsAsync(testrunDto.BranchId))
            {
                var errMsg = $"Branch with id {testrunDto.BranchId} does not exist.";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.NotFound, errMsg);
            }

            try
            {
                var testrun = await _testrun.FindByIdAsync(id);
                if (testrun == null)
                {
                    var errMsg = $"Testrun with id {id} was not found.";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                static DateTimeOffset? ToUtc(DateTimeOffset? t) => t?.ToUniversalTime();
                var startedUtc = ToUtc(testrunDto.StartedAt);
                var finishedUtc = ToUtc(testrunDto.FinishedAt);

                testrun.Version = testrunDto.Version;
                testrun.BranchId = testrunDto.BranchId;

                if (testrunDto.StartedAt.HasValue)
                    testrun.StartedAt = startedUtc;

                if (testrunDto.FinishedAt.HasValue)
                    testrun.FinishedAt = finishedUtc;

                testrun.DurationMs = (testrun.StartedAt.HasValue && testrun.FinishedAt.HasValue)
                    ? (long?)(testrun.FinishedAt.Value - testrun.StartedAt.Value).TotalMilliseconds
                    : null;

                if (testrunDto.EnvironmentJson != null)
                    testrun.EnvironmentJson = testrunDto.EnvironmentJson;

                if (!string.IsNullOrWhiteSpace(testrunDto.IdempotencyKey))
                    testrun.IdempotencyKey = testrunDto.IdempotencyKey;

                _testrun.Update(testrun);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var errMsg = $"Updating the testrun with id {id} failed.";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }

            return RetVal.Ok();
        }

        private bool IsValidDto(CUTestrunDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Version)) 
                return false;

            if (dto.BranchId <= 0) 
                return false;

            if (dto.StartedAt.HasValue
                && dto.FinishedAt.HasValue
                && dto.FinishedAt < dto.StartedAt)
            {
                return false;
            }

            return true;
        }
    }
}
