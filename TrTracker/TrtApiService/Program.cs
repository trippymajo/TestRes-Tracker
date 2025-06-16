using Microsoft.EntityFrameworkCore;
using TrtApiService.App.UploadParsedService;
using TrtApiService.Data;
using TrtApiService.Implementation.Repositories;

namespace TsrUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region APISERVICE
            builder.Services.AddControllers();
            
            builder.Services.AddScoped<UploadParsedWorkflow>();

            builder.Services.AddScoped(typeof(Repository<>));
            builder.Services.AddScoped<BranchRepository>();
            builder.Services.AddScoped<ResultRepository>();
            builder.Services.AddScoped<TestRepository>();
            builder.Services.AddScoped<TestrunRepository>();

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
            #endregion // APISERVICE

            #region SWAGGER
            // Say we are gay gamers exloring reality aka SWAGGER
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion // SWAGGER

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
