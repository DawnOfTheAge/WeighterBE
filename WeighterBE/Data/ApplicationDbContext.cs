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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Configure entities
            modelBuilder.Entity<Weight>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Value).HasPrecision(5, 2);
            });

        }
    }
}
