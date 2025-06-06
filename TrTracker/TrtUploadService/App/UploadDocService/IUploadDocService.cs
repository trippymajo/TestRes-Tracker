namespace TrtUploadService.UploadDocService
{
    public interface IUploadDocService
    {
        /// <summary>
        /// Process async saving file in temporary folder
        /// </summary>
        /// <param name="file">File to save</param>
        /// <returns>
        /// Full file path to saved item. 
        /// [null] - Item was not saved
        /// </returns>
        public Task<string?> SaveFileAsync(IFormFile file);
    }
}
