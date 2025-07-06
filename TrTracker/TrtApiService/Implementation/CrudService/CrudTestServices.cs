using Microsoft.EntityFrameworkCore;
using TrtApiService.App.CrudServices;
using TrtApiService.Data;
using TrtApiService.DTOs;
using TrtApiService.Implementation.Repositories;
using TrtApiService.Models;
using TrtShared.RetValType;

namespace TrtApiService.Implementation.CrudService
{
    public class CrudTestService : ICrudTestService
    {
        private readonly TrtDbContext _context;
        private readonly TestRepository _test;
        private readonly ILogger<CrudTestService> _logger;
        public CrudTestService(TrtDbContext context, TestRepository test, ILogger<CrudTestService> logger) 
        {
            _context = context;
            _test = test;
            _logger = logger;
        }

        public async Task<RetVal<int>> CreateTestAsync(CUTestDTO testDto)
        {
            if (string.IsNullOrWhiteSpace(testDto.Name))
            {
                var errMsg = "Test name is required";
                _logger.LogWarning(errMsg);
                return RetVal<int>.Fail(ErrorType.BadRequest, errMsg);
            }

            try
            {
                if (await _test.IsExistsAsync(testDto.Name))
                {
                    var errMsg = $"Test with name {testDto.Name} already exists";
                    _logger.LogWarning(errMsg);
                    return RetVal<int>.Fail(ErrorType.Conflict, errMsg);
                }

                var test = new Test
                {
                    Name = testDto.Name,
                    Description = testDto.Description
                };

                await _test.CreateAsync(test);
                await _context.SaveChangesAsync();

                return RetVal<int>.Ok(test.Id);
            }
            catch (Exception ex)
            {
                var errMsg = "Creating new test failed";
                _logger.LogError(ex, errMsg);
                return RetVal<int>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> DeleteTestAsync(int id)
        {
            try
            {
                var test = await _test.FindByIdAsync(id);
                if (test == null)
                {
                    var errMsg = $"Test with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                _test.Remove(test);
                await _context.SaveChangesAsync();

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Deleting test with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<Test>> GetTestAsync(int id)
        {
            try
            {
                var test = await _context.Tests.FindAsync(id);
                if (test == null)
                {
                    var errMsg = $"Test with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal<Test>.Fail(ErrorType.NotFound, errMsg);
                }

                return RetVal<Test>.Ok(test);
            }
            catch (Exception ex)
            {
                var errMsg = $"Getting test with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal<Test>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal<IEnumerable<Test>>> GetTestsAsync()
        {
            try
            {
                var tests = await _test.GetAsync();

                return RetVal<IEnumerable<Test>>.Ok(tests);
            }
            catch (Exception ex)
            {
                var errMsg = "Getting all tests failed";
                _logger.LogError(ex, errMsg);
                return RetVal<IEnumerable<Test>>.Fail(ErrorType.ServerError, errMsg);
            }
        }

        public async Task<RetVal> UpdateTestAsync(int id, CUTestDTO testDto)
        {
            if (string.IsNullOrWhiteSpace(testDto.Name))
            {
                var errMsg = "Test name is required";
                _logger.LogWarning(errMsg);
                return RetVal.Fail(ErrorType.BadRequest, errMsg);
            }

            try
            {
                var test = await _test.FindByIdAsync(id);
                if (test == null)
                {
                    var errMsg = $"Test with id {id} was not found";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.NotFound, errMsg);
                }

                if (test.Name != testDto.Name 
                    && await _test.IsExistsAsync(testDto.Name))
                {
                    var errMsg = $"Test with name {testDto.Name} already exists";
                    _logger.LogWarning(errMsg);
                    return RetVal.Fail(ErrorType.Conflict, errMsg);
                }

                _test.Update(test, testDto.Name, testDto.Description);
                await _context.SaveChangesAsync();

                return RetVal.Ok();
            }
            catch (Exception ex)
            {
                var errMsg = $"Updating test with id {id} failed";
                _logger.LogError(ex, errMsg);
                return RetVal.Fail(ErrorType.ServerError, errMsg);
            }
        }
    }
}
