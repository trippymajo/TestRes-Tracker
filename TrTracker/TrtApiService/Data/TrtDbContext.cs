using Microsoft.EntityFrameworkCore;
using TrtApiService.Models;

namespace TrtApiService.Data
{
    public class TrtDbContext : DbContext
    {
        public TrtDbContext(DbContextOptions<TrtDbContext> options)
            : base(options) { }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Testrun> Testruns { get; set; }
        public DbSet<Result> Results { get; set; }
    }
}
