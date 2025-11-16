using Microsoft.EntityFrameworkCore;
using WeighterBE.Models;

namespace WeighterBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Weight> Weights { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Configure entities
            modelBuilder.Entity<Weight>(entity =>
            {
                entity.ToTable("weights");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Value).HasPrecision(5, 2);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
