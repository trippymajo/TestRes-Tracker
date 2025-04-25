using TrtShared.DTO;

namespace TrtUploadService.ResultTransport
{
    public interface IResultTransport
    {
        /// <summary>
        /// Publishes path of the saved file to Parsing service
        /// </summary>
        /// <param name="path">Full file path of the saved file</param>
        public Task PublishPathToFile(string path);

        /// <summary>
        /// Subscrives to Parser service in order to get Parsed Dto
        /// </summary>
        /// <param name="timeout">Timeout value to count</param>
        /// <returns>Dto of the parsed file</returns>
        public Task<TestRunDTO?> SubscribeParsedDto(TimeSpan timeout);
    }
}
