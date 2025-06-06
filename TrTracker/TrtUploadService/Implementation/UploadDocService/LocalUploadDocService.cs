namespace TrtUploadService.UploadDocService
{
    public class LocalUploadDocService : IUploadDocService
    {
        private readonly string _uploadPath;
        private readonly ILogger<LocalUploadDocService> _logger;

        public LocalUploadDocService(ILogger<LocalUploadDocService> logger)
        {
            _uploadPath = Path.Combine(Path.GetTempPath(), "TrtUploads");
            _logger = logger;
        }

        public async Task<string?> SaveFileAsync(IFormFile file)
        {
            var fileExt = Path.GetExtension(file.FileName);
            var newSafeName = $"{Guid.NewGuid()}{fileExt}";
            string? fullFilePath = null;

            // TODO: Dont forget in Docker:
            // RUN mkdir /app/uploads && chmod -R 600 /app/uploads
            // ./TempUploads:/app/uploads:ro  # ro = read-only
            try
            {
                Directory.CreateDirectory(_uploadPath);

                // Process saving
                fullFilePath = Path.Combine(_uploadPath, newSafeName);
                await using var fs = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write);
                await file.CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while saving file to local storage!");
                return null;
            }

            _logger.LogInformation("File was saved: {Path}", fullFilePath);
            return fullFilePath;
        }
    }
}
