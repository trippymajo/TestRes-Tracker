using TrtShared.DTO;

namespace TrtShared.ResultTransport
{
    public interface IUploadTransport
    {
        /// <summary>
        /// Publishes path of the saved file to Parsing service
        /// </summary>
        /// <param name="path">Full file path of the saved file</param>
        public Task PublishPathToFileAsync(string path);

        /// <summary>
        /// Subscribes to Parser service in order to get Parsed Dto
        /// </summary>
        /// <param name="timeout">Timeout value to count</param>
        /// <returns>Dto of the parsed file</returns>
        public Task<TestRunDTO?> GetParsedDtoAsync(TimeSpan timeout);
    }
}
