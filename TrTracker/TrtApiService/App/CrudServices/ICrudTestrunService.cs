using TrtApiService.DTOs;
using TrtApiService.Models;
using TrtShared.RetValType;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudTestrunService
    {
        /// <summary>
        /// Returns an enumerable object with all testruns included in the DB
        /// </summary>
        /// <returns>Enumerable object with all testruns</returns>
        Task<RetVal<IEnumerable<Testrun>>> GetTestrunsAsync();

        /// <summary>
        /// Returns a Testrun object with specified id
        /// </summary>
        /// <param name="id">Id of the testrun</param>
        /// <returns>Testrun model item with specified id</returns>
        Task<RetVal<Testrun>> GetTestrunAsync(int id);

        /// <summary>
        /// Updates a testrun with the specified id and new data
        /// </summary>
        /// <param name="id">Id of the testrun to update</param>
        /// <param name="testrunDto">TestrunDto object with new data</param>
        /// <returns>Result of the update operation</returns>
        Task<RetVal> UpdateTestrunAsync(int id, UpdateTestrunDTO testrunDto);

        /// <summary>
        /// Creates a new testrun from the specified Testrun object
        /// </summary>
        /// <param name="testrunDto">TestrunDto object with data to create</param>
        /// <returns>Created testrun Id</returns>
        Task<RetVal<int>> CreateTestrunAsync(CreateTestrunDTO testrunDto);

        /// <summary>
        /// Deletes the testrun with the specified id
        /// </summary>
        /// <param name="id">Id of the testrun to delete</param>
        /// <returns>Result of the delete operation</returns>
        Task<RetVal> DeleteTestrunAsync(int id);
    }
}