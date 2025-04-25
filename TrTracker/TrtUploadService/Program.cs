using TrtUploadService.UploadService;
using StackExchange.Redis;
using TrtUploadService.ResultTransport;
using TrtUploadService.UploadResultsService;

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
            builder.Services.AddScoped<IResultTransport, RedisTransport>();

            var resultsApiUrl = builder.Configuration["Services:TrtApiService"];
            builder.Services.AddHttpClient<IUploadResultsService, ApiUploadResultsService>(client =>
            {
                client.BaseAddress = new Uri(resultsApiUrl!);
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect("redis"));

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
