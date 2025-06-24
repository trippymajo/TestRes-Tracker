using TrtApiService.Models;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudBranchService
    {
        /// <summary>
        /// Returns an object with all branches included in DB
        /// </summary>
        /// <returns>Enumerable object</returns>
        Task<IEnumerable<Branch>> GetBranchesAsync();

        /// <summary>
        /// Returns a Branch object with specified id
        /// </summary>
        /// <param name="id">Id of the branch</param>
        /// <returns>Branch model item with specified id</returns>
        Task<Branch?> GetBranchAsync(int id);

        /// <summary>
        /// Rename specified with id branch
        /// </summary>
        /// <param name="id">Id of the branch</param>
        /// <param name="strNewName">New name for the branch</param>
        /// <returns>If the branch was renamed</returns>
        Task<bool> RenameBranchAsync(int id, string strNewName);

        /// <summary>
        /// Create new branch from specified Branch object
        /// </summary>
        /// <param name="branch"></param>
        /// <returns>Created branch Id</returns>
        Task<int> CreateBranchAsync(Branch branch);
    }
}
