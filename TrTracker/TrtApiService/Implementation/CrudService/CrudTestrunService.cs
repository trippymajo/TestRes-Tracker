using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
using TrtApiService.Implementation.Repositories;
using TrtApiService.Models;
using TrtShared.RetValType;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<RetVal<int>> CreateTestrunAsync(CreateTestrunDTO testrunDto)
        {
            if (string.IsNullOrWhiteSpace(testrunDto.Version))
                return RetVal<int>.Fail(ErrorType.BadRequest, "Version is required.");

            if (testrunDto.BranchId <= 0)
                return RetVal<int>.Fail(ErrorType.BadRequest, "BranchId must be positive.");

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

        public async Task<RetVal> UpdateTestrunAsync(int id, UpdateTestrunDTO testrunDto)
        {
            if (string.IsNullOrWhiteSpace(testrunDto.Version))
            {
                var errMsg = "Version is required.";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.BadRequest, errMsg);
            }

            if (testrunDto.BranchId <= 0)
            {
                var errMsg = "BranchId must be positive.";
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
    }
}
