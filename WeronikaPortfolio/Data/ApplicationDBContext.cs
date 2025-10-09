using Microsoft.EntityFrameworkCore;
using WeronikaPortfolio.Models;

namespace WeronikaPortfolio.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Message> Messages { get; set; }
    }
}