using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Amazon.S3;

using TrtUploadService.App.UploadDocService;
using TrtUploadService.App.UploadResultsService;
using TrtUploadService.App.ValidatorService;

using TrtUploadService.Implementation.ResultTransport;
using TrtUploadService.Implementation.UploadDocService;
using TrtUploadService.Implementation.UploadResultsService;
using TrtUploadService.Implementation.ValidatorService;

using TrtShared.ServiceCommunication;

namespace TrtUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            #region Validator
            builder.Services.AddScoped<IValidatorService, ValidatorService>();
            #endregion

            #region UPLOAD_SERVICE
            var storageProvider = builder.Configuration["UploadService:StorageProvider"];
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

                builder.Services.AddScoped<IUploadDocService, S3UploadDocService>();
            }
            else
                builder.Services.AddScoped<IUploadDocService, LocalUploadDocService>();
            #endregion // UPLOAD_SERVICE

            #region SWAGGER
            // Ha-ha SWAG rofl
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            #endregion

            #region API_SERVICE
            // Be able to call REST of API service
            var resultsApiUrl = builder.Configuration["Services:TrtApiService"];
            if (string.IsNullOrWhiteSpace(resultsApiUrl))
                throw new InvalidOperationException("Results API URL is missing in configuration");
            builder.Services.AddHttpClient<IUploadResultsService, ApiUploadResultsService>(client =>
            {
                client.BaseAddress = new Uri(resultsApiUrl!);
            });
            #endregion // API_SERVICE

            #region REDIS
            builder.Services.AddSingleton<IUploadTransport, RedisTransport>();
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
