using Microsoft.EntityFrameworkCore;
using TR_Tracker.Data;

namespace TR_Tracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TrtDbContext>(options =>
               options.UseNpgsql(builder.Configuration.GetConnectionString("TrtDbContext") 
               ?? throw new InvalidOperationException("Connection string 'TrtDbContext' not found.")));
            builder.Services.AddControllers();
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
