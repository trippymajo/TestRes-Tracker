using TrtApiService.Models;

namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern extension to work with test table in DB.
    /// </summary>
    public interface ITestRepository : IRepository<Test>
    {
        /// <summary>
        /// CREATE. Creates new sigle test item in test table in DB
        /// </summary>
        /// <param name="testName">Name of the test to put</param>
        /// <param name="testDesc">Description of the test to put</param>
        /// <returns>
        /// Reference to just created Test.
        /// </returns>
        Task<Test> CreateAsync(string testName, string? testDesc = null);

        /// <summary>
        /// CREATE. Creates batched new test items in test table in DB
        /// </summary>
        /// <param name="testName">Name of the test to put</param>
        /// <param name="testDesc">Description of the test to put</param>
        /// <returns>
        /// Reference to just created Test.
        /// </returns>
        Task CreateAsync(IEnumerable<string> testNames);

        /// <summary>
        /// CREATE/READ. Creates new single test or gets test from DB.
        /// </summary>
        /// <param name="testName">Name of the test to get/create</param>
        /// <returns>Reference to just created test</returns>
        Task<Test> GetOrCreateAsync(string testName);

        /// <summary>
        /// CREATE/READ. Creates new single test or gets test from DB.
        /// </summary>
        /// <param name="entity">Test entity to get/create</param>
        /// <returns>Reference to just created test</returns>
        Task<Test> GetOrCreateAsync(Test entity);

        //Task DeleteBranchAsync(); // DELETE
        //Task RenameBranchAsync(string branchName, string newName); // UPDATE
    }
}
