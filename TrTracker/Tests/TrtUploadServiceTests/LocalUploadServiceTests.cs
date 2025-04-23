using Microsoft.AspNetCore.Http;
using System.Text;
using TrtUploadService.UploadService;

namespace TrtUploadService
{
    public class LocalUploadServiceTests
    {
        [Fact]
        public async Task SaveFileAsync_ValidFile_FileIsCreated()
        {
            // Init service
            var uploadService = new LocalUploadService();

            var tempFileName = "test.trx";
            var fileContent = "Hello, world!";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", tempFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/trx"
            };

            // Act
            var result = await uploadService.SaveFileAsync(formFile);

            // Assert
            Assert.NotNull(result);
            var fullPath = Path.Combine(Path.GetTempPath(), "TrtUploads", result);
            Assert.True(File.Exists(fullPath));

            // Clean up
            File.Delete(fullPath);
        }
    }
}
