using Microsoft.EntityFrameworkCore;
using WeronikaPortfolio.Models;

namespace WeronikaPortfolio.Data
{
    public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectImage> ProjectImages { get; set; }
    public DbSet<AboutSection> AboutSections { get; set; }
    public DbSet<Message> Messages { get; set; }
}
}
