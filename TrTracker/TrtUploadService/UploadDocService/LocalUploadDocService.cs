namespace TrtUploadService.UploadService
{
    public class LocalUploadDocService : IUploadDocService
    {
        private readonly string _uploadPath;

        public LocalUploadDocService()
        {
            _uploadPath = Path.Combine(Path.GetTempPath(), "TrtUploads");
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
                Console.WriteLine($"Exception occured while saving file: {ex}");
                return null;
            }

            return fullFilePath;
        }
    }
}
