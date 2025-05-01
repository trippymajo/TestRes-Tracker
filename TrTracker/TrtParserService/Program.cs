using TrtParserService.FileExtensions;
using StackExchange.Redis;
using TrtShared.ServiceCommunication;

namespace TrtParserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Parser>();

            builder.Services.AddScoped<IFileParserFactory, FileParserFactory>();
            builder.Services.AddScoped<TrxParser>();


            // Redis section DI
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
            var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();

            if (redisSettings is null)
                throw new InvalidOperationException("Redis settings are missing in configuration");

            var redisOptions = new ConfigurationOptions
            {
                EndPoints = { $"{redisSettings.Host}:{redisSettings.Port}" },
                Password = redisSettings.Password,
                Ssl = redisSettings.UseSsl
            };


            var host = builder.Build();
            host.Run();
        }
    }
}