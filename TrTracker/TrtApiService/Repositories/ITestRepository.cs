using TrtApiService.Models;

namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern extension to work with test table in DB.
    /// </summary>
    public interface ITestRepository : IRepository<Test>
    {
        /// <summary>
        /// CREATE. Creates new test item in test table in DB
        /// </summary>
        /// <param name="testName">Name of the test to put</param>
        /// <param name="testDesc">Description of the test to put</param>
        /// <returns>
        /// Reference to just created Test.
        /// [null] - Test already exists or the problem occured
        /// </returns>
        Task<Branch?> CreateNewAsync(string testName, string? testDesc = null);

        /// <summary>
        /// CREATE/READ. Creates new branch or gets branch from DB.
        /// </summary>
        /// <param name="branchName">Name of the branch to get/create</param>
        /// <returns>Reference to just created Branch</returns>
        Task<Branch> GetOrCreateAsync(string branchName);

        //Task DeleteBranchAsync(); // DELETE
        //Task RenameBranchAsync(string branchName, string newName); // UPDATE
    }
}
