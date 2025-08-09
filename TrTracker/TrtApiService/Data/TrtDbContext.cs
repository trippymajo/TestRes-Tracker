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
            // TODO: use case insensetive columns in DB for names.

            #region Branch

            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.Name).IsUnique();

            #endregion // Branch


            #region Test

            modelBuilder.Entity<Test>()
                .HasIndex(t => t.Name).IsUnique();
            modelBuilder.Entity<Test>()
                .HasIndex(t => t.ClassName);

            #endregion // Test


            #region TestRun

            // Branch - Testrun Cascade
            modelBuilder.Entity<Testrun>()
                .HasOne(tr => tr.Branch)
                .WithMany(b => b.Testruns)
                .HasForeignKey(tr => tr.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Testrun>()
                .HasIndex(t => new { t.BranchId, t.StartedAt });
            modelBuilder.Entity<Testrun>()
                .HasIndex(t => t.Version);
            modelBuilder.Entity<Testrun>()
                .HasIndex(t => t.IdempotencyKey).IsUnique();

            modelBuilder.Entity<Testrun>()
                .Property(t => t.EnvironmentJson)
                .HasColumnType("jsonb");

            #endregion // TestRun


            #region Result

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

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.TestrunId);
            modelBuilder.Entity<Result>()
                .HasIndex(r => r.TestId);
            modelBuilder.Entity<Result>()
                .HasIndex(r => r.Outcome);

            #endregion // Result
        }
    }
}
