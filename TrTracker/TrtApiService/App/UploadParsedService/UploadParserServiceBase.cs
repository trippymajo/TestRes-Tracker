using TrtApiService.Models;
using TrtShared.DTO;

namespace TrtApiService.App.UploadParsedService
{
    public abstract class UploadParserServiceBase : IUploadParsedService
    {
        public abstract Task<bool> UploadParsedAsync(TestRunDTO dto);

        /// <summary>
        /// Gets list of Tests just added from TestRunDTO
        /// </summary>
        /// <param name="dto">TestRunDTO to make an items from</param>
        /// <returns>List of new tests from Dto</returns>
        protected abstract Task<IList<Test>> GetAddedTestsListFromDtoAsync(TestRunDTO dto);

        /// <summary>
        /// Gets current TestRun model item from TestRunDTO
        /// </summary>
        /// <param name="dto">TestRunDTO to make an item from</param>
        /// <param name="branch">Branch of current testrun</param>
        /// <returns>TestRun entity from Dto</returns>
        protected abstract Testrun GetTestrunFromDto(TestRunDTO dto, Branch branch);

        /// <summary>
        /// Gets curent dto's dictionary Test name - Test Id from DB from TestRunDTO dto
        /// </summary>
        /// <param name="dto">TestRunDTO to make an items from</param>
        /// <returns>Dictionary of Tests names as key and Test Id as value in Db</returns>
        protected abstract Task<IDictionary<string, int>> GetCurTestsDicAsync(TestRunDTO dto);

        /// <summary>
        /// Gets list of Results to create in DB from TestRunDTO
        /// </summary>
        /// <param name="dto">TestRunDTO to make an items from</param>
        /// <param name="testRunId">Id of the current testRun in DB</param>
        /// <param name="testsDic">Dictionary of existing tests in DB</param>
        /// <returns>List of Results from Dto</returns>
        protected abstract IList<Result> GetResultsListFromDto(TestRunDTO dto, int testRunId, IDictionary<string, int> testsDic);
    }
}
