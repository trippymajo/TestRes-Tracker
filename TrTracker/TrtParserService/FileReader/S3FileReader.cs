using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TrtShared.ServiceCommunication;

namespace TrtParserService.FileReader
{
    public class S3FileReader : IFileReader
    {
        private readonly IAmazonS3 _client;
        private readonly IOptions<S3AwsSettings> _awsSettings;
        private readonly ILogger<S3FileReader> _logger;

        public S3FileReader(IAmazonS3 client, IOptions<S3AwsSettings> awsSettings, ILogger<S3FileReader> logger) 
        {
            _client = client;
            _awsSettings = awsSettings;
            _logger = logger;
        }

        public async Task<Stream?> OpenFileStreamAsync(string keyPath)
        {
            if (string.IsNullOrEmpty(keyPath))
            {
                _logger.LogError("Parse failed. Document {KeyPath} is null", keyPath);
                return null;
            }

            var request = new GetObjectRequest
            {
                BucketName = _awsSettings.Value.BucketName,
                Key = keyPath,
            };

            try
            {
                GetObjectResponse response = await _client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "Error getting response for {KeyPath}", keyPath);
                return null;
            }
        }
    }
}
