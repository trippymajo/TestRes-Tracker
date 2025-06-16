using Microsoft.EntityFrameworkCore;
using TrtApiService.App.UploadParsedService;
using TrtApiService.Data;
using TrtApiService.Implementation.Repositories.EfCore;
using TrtApiService.Implementation.UploadParsedService;
using TrtApiService.Repositories;

namespace TsrUploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region APISERVICE
            builder.Services.AddControllers();
            
            // if RelationalDb ...
            builder.Services.AddScoped<IUploadParsedService, EfCoreUploadParsedService>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfCoreRepository<>));
            builder.Services.AddScoped<IBranchRepository, EfCoreBranchRepository>();
            builder.Services.AddScoped<IResultRepository, EfCoreResultRepository>();
            builder.Services.AddScoped<ITestRepository, EfCoreTestRepository>();
            builder.Services.AddScoped<ITestrunRepository, EfCoreTestrunRepository>();
            //endif RelationalDb

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
