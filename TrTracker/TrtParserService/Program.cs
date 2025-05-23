using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Amazon.S3;

using TrtShared.ServiceCommunication;
using TrtParserService.FileExtensions;
using TrtParserService.ResultTransport;
using TrtParserService.FileReader;

namespace TrtParserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            #region PARSER_SERVICE
            builder.Services.AddHostedService<Parser>();
            builder.Services.AddSingleton<IFileParserFactory, FileParserFactory>();

            var storageProvider = builder.Configuration["ParserService:StorageProvider"];
            if (storageProvider == "S3Aws")
            {
                builder.Services.Configure<S3AwsSettings>(builder.Configuration.GetSection("S3Aws"));
                builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>(sp =>
                {
                    var s3AwsSettings = sp.GetRequiredService<IOptions<S3AwsSettings>>().Value;

                    if (s3AwsSettings is null)
                        throw new InvalidOperationException("AWS S3 settings are missing in configuration");

                    var awsRegion = Amazon.RegionEndpoint.GetBySystemName(s3AwsSettings.Region);
                    return new AmazonS3Client(awsRegion);
                });

                // TODO
                builder.Services.AddSingleton<IFileReader, S3FileReader>();
            }
            else
            {
                builder.Services.AddSingleton<IFileReader, LocalFileReader>();
            }

                builder.Services.AddSingleton<TrxParser>();
            #endregion // PARSER_SERVICE

            #region REDIS
            builder.Services.AddSingleton<IParseTransport, RedisTransport>();
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;

                if (redisSettings is null)
                    throw new InvalidOperationException("Redis settings are missing in configuration");

                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { $"{redisSettings.Host}:{redisSettings.Port}" },
                    Password = redisSettings.Password,
                    Ssl = redisSettings.UseSsl
                };
                return ConnectionMultiplexer.Connect(redisOptions);
            });
            #endregion // REDIS

            var host = builder.Build();
            host.Run();
        }
    }
}