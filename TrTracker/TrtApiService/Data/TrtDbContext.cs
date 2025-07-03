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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Branch - Testrun Cascade
            modelBuilder.Entity<Testrun>()
                .HasOne(tr => tr.Branch)
                .WithMany(b => b.Testruns)
                .HasForeignKey(tr => tr.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Testrun - Result Cascade
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Testrun)
                .WithMany(tr => tr.Results)
                .HasForeignKey(r => r.TestrunId)
                .OnDelete(DeleteBehavior.Cascade);

            // Test - Result Restriction/No Action
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Test)
                .WithMany(t => t.Results)
                .HasForeignKey(r => r.TestId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
