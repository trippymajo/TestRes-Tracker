namespace TrtApiService.UploadService
{
    public interface IUploadService
    {
        /// <summary>
        /// Process saving file in temporary folder
        /// </summary>
        /// <param name="file">File to save</param>
        /// <returns>Safe name of the file saved</returns>
        public Task<string?> SaveFileAsync(IFormFile file);
    }
}
