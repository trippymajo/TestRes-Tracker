using TrtShared.Envelope;

namespace TrtUploadService.App.UploadResultsService
{
     public interface IUploadResultsService
    {
        /// <summary>
        /// Pushes/Transferes via https parsed results universal data in to DB
        /// </summary>
        /// <param name="envelope">Universal data to push in to DB</param>
        /// <returns>
        /// [true] - Success
        /// [false] - Something went wrong
        /// </returns>
        public Task<bool> PushResultsToDbAsync(UniEnvelope envelope);
    }
}
