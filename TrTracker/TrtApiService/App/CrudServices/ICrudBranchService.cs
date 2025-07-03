using TrtApiService.Models;

using TrtShared.RetValType;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudBranchService
    {
        /// <summary>
        /// Returns an object with all branches included in DB
        /// </summary>
        /// <returns>Enumerable object</returns>
        Task<RetVal<IEnumerable<Branch>>> GetBranchesAsync();

        /// <summary>
        /// Returns a Branch object with specified id
        /// </summary>
        /// <param name="id">Id of the branch</param>
        /// <returns>Branch model item with specified id</returns>
        Task<RetVal<Branch>> GetBranchAsync(int id);

        /// <summary>
        /// Rename specified with id branch
        /// </summary>
        /// <param name="id">Id of the branch</param>
        /// <param name="strNewName">New name for the branch</param>
        /// <returns>If the branch was renamed</returns>
        Task<RetVal> RenameBranchAsync(int id, string strNewName);

        /// <summary>
        /// Create new branch from specified Branch object
        /// </summary>
        /// <param name="branch">Branch object to create</param>
        /// <returns>Created branch Id</returns>
        Task<RetVal<int>> CreateBranchAsync(Branch branch);

        /// <summary>
        /// Detes branch with all testruns, results related to it
        /// </summary>
        /// <param name="id">Id of the branch to delete</param>
        /// <returns></returns>
        Task<RetVal> DeleteBranchAsync(int id);
    }
}
