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

        public S3UploadDocService(IAmazonS3 s3Client, IOptions<S3AwsSettings> awsSettings)
        {
            _client = s3Client;
            _bucketName = awsSettings.Value.BucketName;
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
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
                return null;
            }
            return newKey;
        }
    }
}
