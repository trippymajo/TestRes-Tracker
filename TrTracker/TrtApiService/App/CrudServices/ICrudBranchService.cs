using TrtApiService.Models;
using TrtApiService.DTOs;

using TrtShared.RetValType;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudBranchService
    {
        /// <summary>
        /// Returns a list with all branches included in DB
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
        /// Updates specified with id branch
        /// </summary>
        /// <param name="id">Id of the branch</param>
        /// <param name="branchDto">BranchDto object with ne </param>
        /// <returns>If the branch was updated</returns>
        Task<RetVal> UpdateBranchAsync(int id, CUBranchDTO branchDto);

        /// <summary>
        /// Create new branch from specified Branch object
        /// </summary>
        /// <param name="branchDto">BranchDto object to create</param>
        /// <returns>Created branch Id</returns>
        Task<RetVal<int>> CreateBranchAsync(CUBranchDTO branchDto);

        /// <summary>
        /// Detes branch with all testruns, results related to it
        /// </summary>
        /// <param name="id">Id of the branch to delete</param>
        /// <returns></returns>
        Task<RetVal> DeleteBranchAsync(int id);
    }
}
