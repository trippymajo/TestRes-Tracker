using TrtParserService.FileExtensions;
using StackExchange.Redis;

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
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect("redis"));

            var host = builder.Build();
            host.Run();
        }
    }
}