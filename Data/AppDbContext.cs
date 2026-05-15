using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Models;

namespace PortfolioCMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<PageVisit> PageVisits { get; set; }
        public DbSet<AboutContent> AboutContent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.Category);
        }
    }
}