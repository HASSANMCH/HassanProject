using HassanProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HassanProject.Models
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

                // Apply pending migrations
                await context.Database.MigrateAsync();

                // Seed Roles
                string[] roleNames = { "Admin", "SuperAdmin", "Manager", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (roleResult.Succeeded)
                        {
                            logger.LogInformation($"Created role: {roleName}");
                        }
                        else
                        {
                            logger.LogError($"Error creating role: {roleName} - {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                // Seed Users
                await SeedUser(userManager, context, "admin@admin.com", "Admin@123", "Admin", logger);
                await SeedUser(userManager, context, "superadmin@admin.com", "SuperAdmin@123", "SuperAdmin", logger);
                await SeedUser(userManager, context, "manager@admin.com", "Manager@123", "Manager", logger);
                await SeedUser(userManager, context, "user@user.com", "User@123", "User", logger);

                // Save changes to the context
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedUser(UserManager<ApplicationUser> userManager, ApplicationDbContext context, string email, string password, string role, ILogger logger)
        {
            if (!context.Users.Any(u => u.UserName == email))
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, role);
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation($"Created user: {email} with role: {role}");
                    }
                    else
                    {
                        logger.LogError($"Error adding user: {email} to role: {role} - {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogError($"Error creating user: {email} - {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"User with email {email} already exists.");
            }
        }
    }
}
