using TrtApiService.Models;

namespace TrtApiService.Repositories
{
    /// <summary>
    /// Repository pattern to work with TestRun table in DB.
    /// </summary>
    public interface ITestrunRepository : IRepository<Testrun>
    {
        //Task DeleteTestRunAsync(); // DELETE
        //Task RenameTestRunAsync( , string newName); // UPDATE
        //Task<Branch> ReadAllTestRuns(); // READ
    }
}
