using Microsoft.EntityFrameworkCore;
using TrtApiService.Data;

namespace TsrUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<TrtDbContext>(options =>
                           options.UseNpgsql(builder.Configuration.GetConnectionString("TrtDbContext")
                           ?? throw new InvalidOperationException("Connection string 'TrtDbContext' not found.")));
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ManageBranches", policy =>
                {
                    policy.RequireClaim("role", "admin");
                });
            });

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
