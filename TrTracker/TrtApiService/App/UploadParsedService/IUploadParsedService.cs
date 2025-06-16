using TrtShared.DTO;

namespace TrtApiService.App.UploadParsedService
{
    public interface IUploadParsedService
    {
        /// <summary>
        /// Upload parsed results to DB from TrtUploadService
        /// </summary>
        /// <param name="dto">Parsed results </param>
        /// <returns>
        /// [true] - Success
        /// [false] - Something went wrong, do HTML 500
        /// </returns>
        Task<bool> UploadParsedAsync(TestRunDTO dto);
    }
}
