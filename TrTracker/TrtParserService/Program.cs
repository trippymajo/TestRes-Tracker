using TrtParserService.FileExtensions;
using StackExchange.Redis;
using TrtShared.ServiceCommunication;
using TrtParserService.ResultTransport;

namespace TrtParserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Parser>();

            builder.Services.AddSingleton<IFileParserFactory, FileParserFactory>();
            builder.Services.AddSingleton<TrxParser>();
            builder.Services.AddSingleton<IParseTransport, RedisTransport>();

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

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisOptions));

            var host = builder.Build();
            host.Run();
        }
    }
}