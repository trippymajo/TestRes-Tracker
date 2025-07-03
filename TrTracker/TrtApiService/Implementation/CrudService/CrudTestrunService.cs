using Microsoft.EntityFrameworkCore;

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
        private readonly ILogger<CrudTestrunService> _logger;

        public CrudTestrunService(TrtDbContext context, ILogger<CrudTestrunService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // WIP
        public async Task<RetVal<int>> CreateTestrunAsync(CreateTestrunDTO testrunDto)
        {
            if (string.IsNullOrWhiteSpace(testrunDto.Version))
                return RetVal<int>.Fail(ErrorType.BadRequest, "Version is required.");

            if (testrunDto.BranchId <= 0)
                return RetVal<int>.Fail(ErrorType.BadRequest, "BranchId must be positive.");

            if (!await IsBranchExists(testrunDto.BranchId))
                return RetVal<int>.Fail(ErrorType.NotFound, $"Branch with id {testrunDto.BranchId} was not found.");

            var testrun = new Testrun
            {
                Version = testrunDto.Version,
                Date = testrunDto.Date,
                BranchId = testrunDto.BranchId
            };

            try
            {
                _context.Testruns.Add(testrun);
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

        // WIP
        public async Task<RetVal> DeleteTestrunAsync(int id)
        {
            if (id <= 0)
                return RetVal.Fail(ErrorType.BadRequest, "Id must be positive.");

            try
            {
                var testrun = await _context.Testruns.FindAsync(id);
                if (testrun == null)
                {
                    var errMsg = $"Testrun with id {id} was not found.";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _context.Testruns.Remove(testrun);
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

        // WIP
        public async Task<RetVal<Testrun>> GetTestrunAsync(int id)
        {
            if (id <= 0)
                return RetVal<Testrun>.Fail(ErrorType.BadRequest, "Id must be positive.");

            try
            {
                var testrun = await _context.Testruns
                    .Include(tr => tr.Branch)
                    .Include(tr => tr.Results)
                    .FirstOrDefaultAsync(tr => tr.Id == id);

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
                var testrunList = await _context.Testruns
                    .Include(tr => tr.Branch)
                    .Include(tr => tr.Results)
                    .ToListAsync();

                return RetVal<IEnumerable<Testrun>>.Ok(testrunList);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting all the testruns failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Testrun>>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        // WIP
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

            try
            {
                var testrun = await _context.Testruns.FindAsync(id);
                if (testrun == null)
                {
                    var errMsg = $"Testrun with id {id} was not found.";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                if (!await IsBranchExists(testrunDto.BranchId))
                {
                    var errMsg = $"Branch with id {testrunDto.BranchId} does not exist.";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                testrun.Version = testrunDto.Version;
                testrun.Date = testrunDto.Date;
                testrun.BranchId = testrunDto.BranchId;

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

        /// <summary>
        /// Checks if the branch with specified id exists
        /// </summary>
        /// <param name="id">id of the branch to check</param>
        /// <returns>
        /// True - branch exists
        /// False - branch does not exists
        /// </returns>
        private async Task<bool> IsBranchExists(int id)
        {
            return await _context.Branches.AnyAsync(b => b.Id == id);
        }
    }
}
