using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Repositories.EfCore
{
    public class EfCoreTestRepository : EfCoreRepository<Test>, ITestRepository
    {
        public EfCoreTestRepository(TrtDbContext context) : base(context) { }

        public async Task<Test> CreateNewAsync(string testName, string? testDesc)
        {
            var test = new Test { Name = testName, Description = testDesc };
            await _context.Tests.AddAsync(test);
            return test;
        }

        public async Task<Test> GetOrCreateAsync(string testName)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(t => t.Name == testName);

            if (test == null)
            {
                test = new Test { Name = testName };
                await _context.Tests.AddAsync(test);
            }

            return test;
        }

        public async Task<Test> GetOrCreateAsync(Test entity)
        {
            var test = await _context.Tests
                .FirstOrDefaultAsync(t => t.Name == entity.Name);

            if (test == null)
            {
                await _context.Tests.AddAsync(entity);
                return entity;
            }

            return test;
        }
    }
}
