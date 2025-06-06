using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;

using Amazon.S3;
using Amazon.S3.Model;

using Moq;

// Tested namespaces
using TrtUploadService.Implementation.UploadDocService;
using TrtShared.ServiceCommunication;


namespace TrtUploadServiceTests
{
    public class S3UploadServiceTests
    {
        IAmazonS3 ConfigureFakeS3() 
        {
            var mockS3 = new Mock<IAmazonS3>();
            mockS3
                .Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                .ReturnsAsync(new PutObjectResponse());

            return mockS3.Object;
        }

        IOptions<S3AwsSettings> ConfigureFakeS3Options()
        {
            var options = Options.Create(new S3AwsSettings { BucketName = "test-bucket", Region = "eu-central-1" });
            return options;
        }

        #region S3UploadDocService
        [Fact]
        public async Task SaveFileAsync_ValidFile_FileIsCreated()
        {
            // Fakes
            var fakeS3 = ConfigureFakeS3();
            var fakeS3Options = ConfigureFakeS3Options();
            var fakeLogger = NullLogger<S3UploadDocService>.Instance;

            // File workaround
            var tempFileName = Path.Combine(AppContext.BaseDirectory, "samples", "mstest.trx");
            using var fileStream = File.OpenRead(tempFileName);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "mstest", tempFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/trx"
            };

            // Init
            var uploadService = new S3UploadDocService(fakeS3, fakeS3Options, fakeLogger);

            // Act
            var result = await uploadService.SaveFileAsync(formFile);

            // Assert
            Assert.NotNull(result);
        }
        #endregion // S3UploadDocService
    }
}
