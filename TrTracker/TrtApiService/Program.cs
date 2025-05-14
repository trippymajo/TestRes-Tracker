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

            // Say we are gay gamers exloring reality aka SWAGGER
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
