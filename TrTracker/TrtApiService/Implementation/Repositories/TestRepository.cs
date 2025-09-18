using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;
using TrtApiService.Models;

namespace TrtApiService.Implementation.Repositories
{
    public class TestRepository : Repository<Test>
    {
        public TestRepository(TrtDbContext context) : base(context) { }

        /// <summary>
        /// CREATE by testName with/without Description
        /// </summary>
        /// <param name="testName">Test name to create</param>
        /// <param name="testDesc">Description for the test to provide</param>
        /// <returns>Created Test</returns>
        public async Task<Test> CreateAsync(string testName, string? testDesc = null)
        {
            var test = new Test { Name = testName, Description = testDesc };
            await _context.Tests.AddAsync(test);
            return test;
        }

        /// <summary>
        /// CREATE BATCH with enumerable object of test names
        /// </summary>
        /// <param name="testNames">Enumerable object with test names</param>
        public async Task CreateAsync(IEnumerable<string> testNames)
        {
            var tests = testNames
                .Select(tn => new Test { Name = tn })
                .ToList();

            await _context.AddRangeAsync(tests);
        }

        /// <summary>
        /// READ OR CREATE by test name
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// READ OR CREATE
        /// </summary>
        /// <param name="entity">Test Entity</param>
        /// <returns>Existing or created Test</returns>
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

        /// <summary>
        /// READ. Checks if the test with specified name already exists
        /// </summary>
        /// <param name="name">Test name to check</param>
        /// <returns>
        /// True - test exists
        /// False - no such test found
        ///</returns>
        public async Task<bool> IsExistsAsync(string name)
        {
            return await _context.Tests.AnyAsync(t => t.Name == name);
        }

        /// <summary>
        /// READ. Checks if the test with specified id already exists
        /// </summary>
        /// <param name="id">Test id to check</param>
        /// <returns>
        /// True - test exists
        /// False - no such test found
        ///</returns>
        public async Task<bool> IsExistsAsync(int id)
        {
            return await _context.Tests.AnyAsync(t => t.Id == id);
        }

        /// <summary>
        /// UPDATE. Changes name of the current test
        /// </summary>
        /// <param name="test">Test to update</param>
        /// <param name="className">Class name update</param>
        /// <param name="newName">Name to save for test</param>
        public void Update(Test test, string newName, string? className = null, string? desc = null)
        {
            test.Name = newName;
            test.ClassName = className;
            test.Description = desc;
        }
    }
}
