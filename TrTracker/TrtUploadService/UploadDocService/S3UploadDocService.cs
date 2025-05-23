using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using TrtShared.ServiceCommunication;
using TrtUploadService.UploadService;

namespace TrtUploadService.UploadDocService
{
    public class S3UploadDocService : IUploadDocService
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _client;
        private readonly ILogger<S3UploadDocService> _logger;

        public S3UploadDocService(IAmazonS3 s3Client, IOptions<S3AwsSettings> awsSettings, ILogger<S3UploadDocService> logger)
        {
            _client = s3Client;
            _bucketName = awsSettings.Value.BucketName;
            _logger = logger;
        }

        public async Task<string?> SaveFileAsync(IFormFile file)
        {
                var fileExt = Path.GetExtension(file.FileName);
                var newKey = $"{Guid.NewGuid()}" + fileExt;
            try
            {
                using var ms = file.OpenReadStream();
                // 1. Put object-specify only key name for the new object.
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = newKey,
                    InputStream = ms,
                    ContentType = file.ContentType
                };

                PutObjectResponse response = await _client.PutObjectAsync(putRequest);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Error encountered on saving file to S3");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error encountered when saving file to S3");
                return null;
            }

            _logger.LogInformation("File was saved in S3: {Key}", newKey);
            return newKey;
        }
    }
}
