
using Microsoft.EntityFrameworkCore;
using WeighterBE.Models;

namespace WeighterBE.Data
{
    public class ReportsDbContext : DbContext
    {
        public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");  // SQL Server typically uses PascalCase
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}

