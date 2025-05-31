using TrtApiService.Models;

namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern extension to work with Branch table in DB.
    /// </summary>
    public interface IBranchRepository : IRepository<Branch>
    {
        /// <summary>
        /// CREATE. Creates new branch in Branch table in DB
        /// </summary>
        /// <param name="branchName">Name of the branch to put</param>
        /// <returns>
        /// Reference to just created Branch.
        /// [null] - Branch already exists or the problem occured
        /// </returns>
        Task<Branch?> CreateNewAsync(string branchName);

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
