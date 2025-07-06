using TrtApiService.DTOs;
using TrtApiService.Models;
using TrtShared.RetValType;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudTestService
    {
        /// <summary>
        /// Returns a list with all tests included in DB
        /// </summary>
        /// <returns>Enumerable object with tests</returns>
        Task<RetVal<IEnumerable<Test>>> GetTestsAsync();

        /// <summary>
        /// Returns a Test object with specified id
        /// </summary>
        /// <param name="id">Id of the test</param>
        /// <returns>Test model item with specified id</returns>
        Task<RetVal<Test>> GetTestAsync(int id);

        /// <summary>
        /// Updates specified with id test
        /// </summary>
        /// <param name="id">Id of the test</param>
        /// <param name="testDto">TestDto object with ne </param>
        /// <returns>If the Test was updated</returns>
        Task<RetVal> UpdateTestAsync(int id, CUTestDTO testDto);

        /// <summary>
        /// Create new Test from specified Test object
        /// </summary>
        /// <param name="testDto">TestDto object to create</param>
        /// <returns>Created Test Id</returns>
        Task<RetVal<int>> CreateTestAsync(CUTestDTO testDto);

        /// <summary>
        /// Detes Test with all testruns, results related to it
        /// </summary>
        /// <param name="id">Id of the Test to delete</param>
        /// <returns></returns>
        Task<RetVal> DeleteTestAsync(int id);
    }
}
