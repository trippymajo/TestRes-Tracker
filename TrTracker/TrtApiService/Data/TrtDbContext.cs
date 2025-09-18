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

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            // TODO: use case insensetive columns in DB for names.

            // === Branch ===
            mb.Entity<Branch>()
                .HasIndex(b => b.Name)
                .IsUnique();

            // === Test ===
            mb.Entity<Test>()
                .HasIndex(t => t.Name).IsUnique();
            mb.Entity<Test>()
                .HasIndex(t => t.ClassName);


            // === TestRun ===
            // Branch -> Testrun Cascade
            mb.Entity<Testrun>()
                .HasOne(tr => tr.Branch)
                .WithMany(b => b.Testruns)
                .HasForeignKey(tr => tr.BranchId)
                .OnDelete(DeleteBehavior.Cascade); // Change to restrict in the future

            mb.Entity<Testrun>()
                .HasIndex(t => t.IdempotencyKey).IsUnique();
            mb.Entity<Testrun>()
                .HasIndex(t => new { t.BranchId, t.StartedAt });
            mb.Entity<Testrun>()
                .HasIndex(t => t.Version);

            mb.Entity<Testrun>()
                .Property(t => t.EnvironmentJson)
                .HasColumnType("jsonb");

            // === Result ===
            // Testrun - Result Cascade
            mb.Entity<Result>()
                .HasOne(r => r.Testrun)
                .WithMany(tr => tr.Results)
                .HasForeignKey(r => r.TestrunId)
                .OnDelete(DeleteBehavior.Cascade); // Change to restrict in the future

            // Test - Result Restriction/No Action
            mb.Entity<Result>()
                .HasOne(r => r.Test)
                .WithMany(t => t.Results)
                .HasForeignKey(r => r.TestId)
                .OnDelete(DeleteBehavior.NoAction);

            mb.Entity<Result>()
                .HasIndex(r => r.TestrunId);
            mb.Entity<Result>()
                .HasIndex(r => r.TestId);
            mb.Entity<Result>()
                .HasIndex(r => r.Outcome);
        }
    }
}
