using CarPairs.Core;
using Microsoft.EntityFrameworkCore;

namespace CarPairs.Core
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Part> Parts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Engine" },
                new Category { Id = 2, Name = "Suspension" },
                new Category { Id = 3, Name = "Brakes" }
            );

            // Seed Manufacturers
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { Id = 1, Name = "Bosch" },
                new Manufacturer { Id = 2, Name = "Delphi" },
                new Manufacturer { Id = 3, Name = "Valeo" }
            );
        }
    }
}
