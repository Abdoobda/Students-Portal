using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options): base(Options)
        { 
        }

        public DbSet<Student> Students { get; set; }

        // ADD THIS METHOD - REQUIRED FOR MIGRATIONS
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connString = config.GetConnectionString("StudentPortal");
                optionsBuilder.UseSqlServer(connString);
            }
        }
    }
}
