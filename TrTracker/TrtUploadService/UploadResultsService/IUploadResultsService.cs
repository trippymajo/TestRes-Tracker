using TrtShared.DTO;

namespace TrtUploadService.UploadResultsService
{
     public interface IUploadResultsService
    {
        /// <summary>
        /// Pushes parsed results Dto in to DB
        /// </summary>
        /// <param name="dto">DTO to push in to DB</param>
        /// <returns>
        /// [true] - Success
        /// [false] - Something went wrong
        /// </returns>
        public Task<bool> PushResultsToDb(TestRunDTO dto);
    }
}
