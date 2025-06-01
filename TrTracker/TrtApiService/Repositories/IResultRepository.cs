using TrtApiService.Models;

namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern to work with Result table in DB.
    /// </summary>
    public interface IResultRepository : IRepository<Result>
    {
        //Task DeleteBranchAsync(); // DELETE
        //Task RenameBranchAsync( , string newName); // UPDATE
        //Task<Branch> ReadAllBranchNames(); // READ
    }
}
