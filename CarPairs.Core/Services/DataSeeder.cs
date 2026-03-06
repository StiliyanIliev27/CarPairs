using CarPairs.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CarPairs.Core.Services
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles if not exist
            string[] roles = { "Admin", "Manager", "User", "Guest" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin user
            var adminEmail = "admin@carpairs.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                await userManager.CreateAsync(adminUser, "AdminPass123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Manager users
            var managerEmails = new[] { "manager1@carpairs.com", "manager2@carpairs.com" };
            foreach (var email in managerEmails)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var managerUser = new ApplicationUser { UserName = email, Email = email };
                    await userManager.CreateAsync(managerUser, "ManagerPass123!");
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }

            // Seed some User users
            var userEmails = new[] { "user1@carpairs.com", "user2@carpairs.com", "user3@carpairs.com" };
            foreach (var email in userEmails)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser { UserName = email, Email = email };
                    await userManager.CreateAsync(user, "UserPass123!");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }

        public static async Task SeedCategories(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category
                    {
                        Name = "Engine Parts",
                        Description = "Components related to the engine system",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Transmission",
                        Description = "Transmission system components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Brake System",
                        Description = "Brake components and accessories",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Suspension",
                        Description = "Suspension and steering components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Electrical",
                        Description = "Electrical and electronic components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Body Parts",
                        Description = "Exterior and interior body components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Exhaust System",
                        Description = "Exhaust and emission control components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Cooling System",
                        Description = "Radiators, hoses, and cooling components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Fuel System",
                        Description = "Fuel injection and delivery components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Air Conditioning",
                        Description = "HVAC and climate control components",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedManufacturers(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Manufacturers.Any())
            {
                var manufacturers = new[]
                {
                    new Manufacturer
                    {
                        Name = "Bosch",
                        Country = "Germany",
                        FoundedYear = 1886,
                        Website = "https://www.bosch.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Continental",
                        Country = "Germany",
                        FoundedYear = 1871,
                        Website = "https://www.continental.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Denso",
                        Country = "Japan",
                        FoundedYear = 1949,
                        Website = "https://www.denso.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Delphi Technologies",
                        Country = "United Kingdom",
                        FoundedYear = 1994,
                        Website = "https://www.delphi.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Magna International",
                        Country = "Canada",
                        FoundedYear = 1957,
                        Website = "https://www.magna.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "ZF Friedrichshafen",
                        Country = "Germany",
                        FoundedYear = 1915,
                        Website = "https://www.zf.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "TRW Automotive",
                        Country = "United States",
                        FoundedYear = 1904,
                        Website = "https://www.trw.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Valeo",
                        Country = "France",
                        FoundedYear = 1923,
                        Website = "https://www.valeo.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Hella",
                        Country = "Germany",
                        FoundedYear = 1899,
                        Website = "https://www.hella.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Mahle",
                        Country = "Germany",
                        FoundedYear = 1920,
                        Website = "https://www.mahle.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Brembo",
                        Country = "Italy",
                        FoundedYear = 1961,
                        Website = "https://www.brembo.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Schaeffler Group",
                        Country = "Germany",
                        FoundedYear = 1946,
                        Website = "https://www.schaeffler.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Manufacturers.AddRangeAsync(manufacturers);
                await context.SaveChangesAsync();
            }
        }
    }
}