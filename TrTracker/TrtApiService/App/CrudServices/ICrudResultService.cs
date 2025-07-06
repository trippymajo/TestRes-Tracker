using TrtApiService.DTOs;
using TrtApiService.Models;

using TrtShared.RetValType;

namespace TrtApiService.App.CrudServices
{
    public interface ICrudResultService
    {
        /// <summary>
        /// Returns a list of all test results in the DB
        /// </summary>
        /// <returns>Enumerable of Result objects</returns>
        Task<RetVal<IEnumerable<Result>>> GetResultsAsync();

        /// <summary>
        /// Returns a Result object by its id
        /// </summary>
        /// <param name="id">Result id</param>
        /// <returns>Result object with specified id.</returns>
        Task<RetVal<Result>> GetResultAsync(int id);

        /// <summary>
        /// Updates a Result by id
        /// </summary>
        /// <param name="id">Result id</param>
        /// <param name="resultDto">DTO with new result data</param>
        /// <returns>Status of the update operation</returns>
        Task<RetVal> UpdateResultAsync(int id, UpdateResultDTO resultDto);

        /// <summary>
        /// Creates a new Result from the provided DTO
        /// </summary>
        /// <param name="resultDto">DTO to create the result from</param>
        /// <returns>Created Result id</returns>
        Task<RetVal<int>> CreateResultAsync(CreateResultDTO resultDto);

        /// <summary>
        /// Deletes a result with the given id
        /// </summary>
        /// <param name="id">Id of the result to delete</param>
        /// <returns>Status of the delete operation</returns>
        Task<RetVal> DeleteResultAsync(int id);
    }
}
