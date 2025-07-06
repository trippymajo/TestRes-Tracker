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

        public Task<RetVal<int>> CreateTestAsync(CUTestDTO testDto)
        {
            throw new NotImplementedException();
        }

        public Task<RetVal> DeleteTestAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RetVal<Test>> GetTestAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RetVal<IEnumerable<Test>>> GetTestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RetVal> UpdateTestAsync(int id, CUTestDTO testDto)
        {
            throw new NotImplementedException();
        }
    }
}
