using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

// Tested namespaces
using TrtUploadService.Implementation.UploadDocService;

namespace TrtUploadServiceTests
{
    public class LocalUploadServiceTests
    {
        #region LocalUploadDocService
        [Fact]
        public async Task SaveFileAsync_ValidFile_FileIsCreated()
        {
            // Init service
            var fakeLogger = NullLogger<LocalUploadDocService>.Instance;
            var uploadService = new LocalUploadDocService(fakeLogger);

            // File workaround
            var tempFileName = Path.Combine(AppContext.BaseDirectory, "samples", "mstest.trx");
            using var fileStream = File.OpenRead(tempFileName);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "mstest", tempFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/trx"
            };

            // Act
            var result = await uploadService.SaveFileAsync(formFile);

            // Assert
            Assert.NotNull(result);
            var savedFullPath = Path.Combine(Path.GetTempPath(), "TrtUploads", result);
            Assert.True(File.Exists(savedFullPath));

            // Clean up
            File.Delete(savedFullPath);
        }
        #endregion // LocalUploadDocService
    }
}
