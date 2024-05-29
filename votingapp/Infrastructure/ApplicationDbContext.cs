using votingapp.Models;
using Microsoft.EntityFrameworkCore;

namespace votingapp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Voter> voters { get; set; }
        public DbSet<Candidate> candidates { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voter>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.has_voted).IsRequired();
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.votes).IsRequired().HasDefaultValue(0);
            });
        }

    }
}
