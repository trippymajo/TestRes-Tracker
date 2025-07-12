using TrtShared.Envelope;

namespace TrtShared.ServiceCommunication
{
    public interface IUploadTransport
    {
        /// <summary>
        /// Publishes path of the saved file to Parsing service
        /// </summary>
        /// <param name="path">Full file path of the saved file</param>
        public Task PublishPathToFileAsync(string path);

        /// <summary>
        /// Subscribes to Parser service in order to get universal envelope data
        /// </summary>
        /// <param name="timeout">Timeout value to count</param>
        /// <returns>universal envelope data of the parsed file</returns>
        public Task<UniEnvelope?> GetParsedDataAsync(TimeSpan timeout);
    }
}
