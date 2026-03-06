using CarPairs.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarPairs.Core.Services
{
    public static class DataSeeder
    {
        public static async Task SeedOrganizations(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Organizations.Any())
            {
                var organizations = new[]
                {
                    new Organization
                    {
                        Name = "AutoCare Solutions",
                        Description = "Leading automotive parts distributor",
                        ContactEmail = "contact@autocare.com",
                        PhoneNumber = "+1-555-0101",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Organization
                    {
                        Name = "speedTech Parts",
                        Description = "High-performance automotive components",
                        ContactEmail = "contact@speedtech.com",
                        PhoneNumber = "+1-555-0102",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Organization
                    {
                        Name = "Global Parts Warehouse",
                        Description = "International parts supplier",
                        ContactEmail = "contact@globalparts.com",
                        PhoneNumber = "+1-555-0103",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                };

                await context.Organizations.AddRangeAsync(organizations);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed system admin (no organization)
            var adminEmail = "admin@carpairs.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser 
                { 
                    UserName = adminEmail, 
                    Email = adminEmail,
                    OrganizationId = null, // System admin
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await userManager.CreateAsync(adminUser, "AdminPass123!");
            }

            // Get organizations for seeding
            var orgs = await context.Organizations.ToListAsync();
            if (orgs.Count < 2)
                return; // Organizations not seeded yet

            var org1 = orgs[0]; // AutoCare Solutions
            var org2 = orgs[1]; // speedTech Parts

            // Seed Manager for org 1
            var manager1Email = "manager1@autocare.com";
            if (await userManager.FindByEmailAsync(manager1Email) == null)
            {
                var manager1 = new ApplicationUser
                {
                    UserName = manager1Email,
                    Email = manager1Email,
                    OrganizationId = org1.Id,
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await userManager.CreateAsync(manager1, "ManagerPass123!");
            }

            // Seed Manager for org 2
            var manager2Email = "manager1@speedtech.com";
            if (await userManager.FindByEmailAsync(manager2Email) == null)
            {
                var manager2 = new ApplicationUser
                {
                    UserName = manager2Email,
                    Email = manager2Email,
                    OrganizationId = org2.Id,
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await userManager.CreateAsync(manager2, "ManagerPass123!");
            }

            // Seed regular Users for org 1
            var userEmails1 = new[] 
            { 
                ("user1@autocare.com", "John Doe"),
                ("user2@autocare.com", "Jane Smith"),
                ("accountant@autocare.com", "Robert Johnson")
            };
            foreach (var (email, name) in userEmails1)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var role = email.Contains("accountant") ? UserRole.Guest : UserRole.User;
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        OrganizationId = org1.Id,
                        Role = role,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await userManager.CreateAsync(user, "UserPass123!");
                }
            }

            // Seed regular Users for org 2
            var userEmails2 = new[] 
            { 
                ("user1@speedtech.com", "Mike Wilson"),
                ("accountant@speedtech.com", "Sarah Davis")
            };
            foreach (var (email, name) in userEmails2)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var role = email.Contains("accountant") ? UserRole.Guest : UserRole.User;
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        OrganizationId = org2.Id,
                        Role = role,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await userManager.CreateAsync(user, "UserPass123!");
                }
            }
        }

        public static async Task SeedCategories(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Categories.Any())
            {
                var orgs = await context.Organizations.ToListAsync();
                if (orgs.Count == 0)
                    return;

                var org1 = orgs[0];
                var org2 = orgs.Count > 1 ? orgs[1] : orgs[0];

                var categories = new[]
                {
                    new Category
                    {
                        Name = "Engine Parts",
                        Description = "Components related to the engine system",
                        OrganizationId = org1.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Transmission",
                        Description = "Transmission system components",
                        OrganizationId = org1.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Brake System",
                        Description = "Brake components and accessories",
                        OrganizationId = org1.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Suspension",
                        Description = "Suspension and steering components",
                        OrganizationId = org1.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Electrical",
                        Description = "Electrical and electronic components",
                        OrganizationId = org1.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Cooling System",
                        Description = "Radiators, hoses, and cooling components",
                        OrganizationId = org2.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Fuel System",
                        Description = "Fuel injection and delivery components",
                        OrganizationId = org2.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Category
                    {
                        Name = "Performance Parts",
                        Description = "High-performance upgrades and components",
                        OrganizationId = org2.Id,
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
                var orgs = await context.Organizations.ToListAsync();
                if (orgs.Count == 0)
                    return;

                var org1 = orgs[0];
                var org2 = orgs.Count > 1 ? orgs[1] : orgs[0];

                var manufacturers = new[]
                {
                    new Manufacturer
                    {
                        Name = "Bosch",
                        Country = "Germany",
                        FoundedYear = 1886,
                        Website = "https://www.bosch.com",
                        OrganizationId = org1.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Continental",
                        Country = "Germany",
                        FoundedYear = 1871,
                        Website = "https://www.continental.com",
                        OrganizationId = org1.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Denso",
                        Country = "Japan",
                        FoundedYear = 1949,
                        Website = "https://www.denso.com",
                        OrganizationId = org1.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Brembo",
                        Country = "Italy",
                        FoundedYear = 1961,
                        Website = "https://www.brembo.com",
                        OrganizationId = org2.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Valeo",
                        Country = "France",
                        FoundedYear = 1923,
                        Website = "https://www.valeo.com",
                        OrganizationId = org2.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Manufacturer
                    {
                        Name = "Mahle",
                        Country = "Germany",
                        FoundedYear = 1920,
                        Website = "https://www.mahle.com",
                        OrganizationId = org2.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Manufacturers.AddRangeAsync(manufacturers);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedParts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Parts.Any())
            {
                var parts = new List<Part>();

                // Get org1 data
                var org1 = await context.Organizations.FirstOrDefaultAsync();
                if (org1 != null)
                {
                    var engineCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Engine Parts" && c.OrganizationId == org1.Id);
                    var transmissionCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Transmission" && c.OrganizationId == org1.Id);
                    var brakesCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Brake System" && c.OrganizationId == org1.Id);

                    var bosch = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Bosch" && m.OrganizationId == org1.Id);
                    var continental = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Continental" && m.OrganizationId == org1.Id);
                    var denso = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Denso" && m.OrganizationId == org1.Id);

                    if (engineCategory != null && bosch != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "Spark Plug Set",
                            Price = 45.99m,
                            StockQuantity = 150,
                            ManufacturerId = bosch.Id,
                            CategoryId = engineCategory.Id,
                            OrganizationId = org1.Id,
                            CreatedAt = DateTime.UtcNow
                        });

                        parts.Add(new Part
                        {
                            Name = "Oil Filter",
                            Price = 12.50m,
                            StockQuantity = 200,
                            ManufacturerId = bosch.Id,
                            CategoryId = engineCategory.Id,
                            OrganizationId = org1.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    if (transmissionCategory != null && continental != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "Transmission Fluid",
                            Price = 35.00m,
                            StockQuantity = 100,
                            ManufacturerId = continental.Id,
                            CategoryId = transmissionCategory.Id,
                            OrganizationId = org1.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    if (brakesCategory != null && denso != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "Brake Pads (Front)",
                            Price = 89.99m,
                            StockQuantity = 75,
                            ManufacturerId = denso.Id,
                            CategoryId = brakesCategory.Id,
                            OrganizationId = org1.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                // Get org2 data
                var org2 = await context.Organizations.Skip(1).FirstOrDefaultAsync();
                if (org2 != null)
                {
                    var coolingCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Cooling System" && c.OrganizationId == org2.Id);
                    var fuelCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Fuel System" && c.OrganizationId == org2.Id);
                    var performanceCategory = await context.Categories
                        .FirstOrDefaultAsync(c => c.Name == "Performance Parts" && c.OrganizationId == org2.Id);

                    var brembo = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Brembo" && m.OrganizationId == org2.Id);
                    var valeo = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Valeo" && m.OrganizationId == org2.Id);
                    var mahle = await context.Manufacturers
                        .FirstOrDefaultAsync(m => m.Name == "Mahle" && m.OrganizationId == org2.Id);

                    if (coolingCategory != null && valeo != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "Racing Radiator",
                            Price = 299.99m,
                            StockQuantity = 20,
                            ManufacturerId = valeo.Id,
                            CategoryId = coolingCategory.Id,
                            OrganizationId = org2.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    if (performanceCategory != null && brembo != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "Performance Brake Kit",
                            Price = 599.99m,
                            StockQuantity = 15,
                            ManufacturerId = brembo.Id,
                            CategoryId = performanceCategory.Id,
                            OrganizationId = org2.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    if (fuelCategory != null && mahle != null)
                    {
                        parts.Add(new Part
                        {
                            Name = "High-Flow Fuel Injector",
                            Price = 159.99m,
                            StockQuantity = 40,
                            ManufacturerId = mahle.Id,
                            CategoryId = fuelCategory.Id,
                            OrganizationId = org2.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (parts.Any())
                {
                    await context.Parts.AddRangeAsync(parts);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}