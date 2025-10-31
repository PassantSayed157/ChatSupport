using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options) { }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .Property(a => a.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Team>()
                .HasMany(t => t.Agents)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
