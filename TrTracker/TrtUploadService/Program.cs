using TrtUploadService.UploadService;
using StackExchange.Redis;
using TrtUploadService.ResultTransport;
using TrtUploadService.UploadResultsService;
using TrtShared.ServiceCommunication;

namespace TrtUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IUploadDocService, LocalUploadDocService>();
            builder.Services.AddSingleton<IUploadTransport, RedisTransport>();


            // Be able to call REST of API service
            var resultsApiUrl = builder.Configuration["Services:TrtApiService"];
            builder.Services.AddHttpClient<IUploadResultsService, ApiUploadResultsService>(client =>
            {
                client.BaseAddress = new Uri(resultsApiUrl!);
            });


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


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
