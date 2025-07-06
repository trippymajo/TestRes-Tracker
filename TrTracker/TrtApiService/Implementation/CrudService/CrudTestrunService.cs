using TrtApiService.Implementation.Repositories;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
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
            // Validation
            if (!IsValidDto(testrunDto))
            {
                var errMsg = "Testrun data provided is not valid";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            if (!await _branch.IsExistsAsync(testrunDto.BranchId))
                return RetVal<int>.Fail(ErrorType.NotFound, $"Branch with id {testrunDto.BranchId} was not found.");

            var exists = await _testrun.IsExistsAsync(testrunDto.BranchId, testrunDto.Version, testrunDto.Date);

            if (exists)
            {
                var errMsg = "Such testrun already is in the DB";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.Conflict, errMsg);
            }

            var testrun = new Testrun
            {
                Version = testrunDto.Version,
                Date = testrunDto.Date.Kind == DateTimeKind.Utc
                    ? testrunDto.Date
                    : testrunDto.Date.ToUniversalTime(),
                BranchId = testrunDto.BranchId
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
            // Validation
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

                _testrun.Update(testrun, testrunDto.Version, testrunDto.Date, testrunDto.BranchId);
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
            if (string.IsNullOrWhiteSpace(dto.Version)
                || dto.Date != default
                || dto.BranchId <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
