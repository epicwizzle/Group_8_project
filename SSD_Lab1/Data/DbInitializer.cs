using Microsoft.AspNetCore.Identity;
using SSD_Lab1.Data;
using SSD_Lab1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSD_Lab1.Data
{
    public static class DbInitializer
    {
        public static string DemoPassword;

        public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            context.Database.Migrate();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (roleManager.Roles.Count() > 0)
            {
                logger.LogInformation("Database already initialized with roles");
                return 1;
            }

            int result = await SeedRoles(roleManager, logger);
            if (result != 0)
            {
                logger.LogError("Role seeding failed");
                return 2;
            }

            if (userManager.Users.Count() > 0)
            {
                logger.LogInformation("Users already exist in database");
                return 3;
            }

            result = await SeedUsers(userManager, logger);
            if (result != 0)
            {
                logger.LogError("User seeding failed");
                return 4;
            }

            result = await SeedCompanies(context, logger);
            if (result != 0)
            {
                logger.LogError("Company seeding failed");
                return 5;
            }

            logger.LogInformation("Database initialization completed successfully");
            return 0;
        }

        private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            var result = await roleManager.CreateAsync(new IdentityRole("Supervisor"));
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create Supervisor role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 1;
            }

            result = await roleManager.CreateAsync(new IdentityRole("Employee"));
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create Employee role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 2;
            }

            logger.LogInformation("Roles seeded successfully");
            return 0;
        }

        private static async Task<int> SeedUsers(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var supervisor = new ApplicationUser
            {
                UserName = "the.supervisor@mohawkcollege.ca",
                Email = "the.supervisor@mohawkcollege.ca",
                FirstName = "John",
                LastName = "Supervisor",
                PhoneNumber = "555-0101",
                City = "Toronto",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(supervisor, DemoPassword);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create supervisor user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 1;
            }

            result = await userManager.AddToRoleAsync(supervisor, "Supervisor");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to assign Supervisor role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 2;
            }

            var employee = new ApplicationUser
            {
                UserName = "the.employee@mohawkcollege.ca",
                Email = "the.employee@mohawkcollege.ca",
                FirstName = "Jane",
                LastName = "Employee",
                PhoneNumber = "555-0102",
                City = "Montreal",
                EmailConfirmed = true
            };
            
            result = await userManager.CreateAsync(employee, DemoPassword);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create employee user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 3;
            }

            result = await userManager.AddToRoleAsync(employee, "Employee");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to assign Employee role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return 4;
            }

            logger.LogInformation("Users seeded successfully");
            return 0;
        }

        private static async Task<int> SeedCompanies(ApplicationDbContext context, ILogger logger)
        {
            if (context.Companies.Any())
            {
                logger.LogInformation("Companies already exist in database");
                return 1;
            }

            var companies = new List<Company>
            {
                new Company
                {
                    Name = "Shopify Inc",
                    YearsInBusiness = 18,
                    Website = "https://shopify.com",
                    Province = "Ontario"
                },
                new Company
                {
                    Name = "BlackBerry Limited",
                    YearsInBusiness = 40,
                    Website = "https://blackberry.com",
                    Province = "Ontario"
                },
                new Company
                {
                    Name = "Magna International Inc",
                    YearsInBusiness = 66,
                    Website = "https://magna.com",
                    Province = "Ontario"
                },
                new Company
                {
                    Name = "Manulife Financial",
                    YearsInBusiness = 137,
                    Website = "https://manulife.com",
                    Province = "Ontario"
                },
                new Company
                {
                    Name = "Canadian Tire Corporation",
                    YearsInBusiness = 101,
                    Website = "https://canadiantire.ca",
                    Province = "Ontario"
                }
            };

            try
            {
                context.Companies.AddRange(companies);
                await context.SaveChangesAsync();
                logger.LogInformation("Companies seeded successfully");
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while saving companies");
                return 2;
            }
        }
    }
}
