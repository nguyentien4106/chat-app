using EzyChat.Domain.Entities;
using EzyChat.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EzyChat.Infrastructure;

public class EzyChatDbInitializer(
    EzyChatDbContext context,
    ILogger<EzyChatDbInitializer> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager
)
{
    // Known GUIDs from init_data.sql for consistency
    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsNpgsql())
            {
                // Check if there are any pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("Database is up to date. No pending migrations.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedNormalUsersAsync();

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    #region Seed Roles
    
    private async Task SeedRolesAsync()
    {
        var roles = new[] { nameof(Roles.Admin), nameof(Roles.User) };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>(roleName);
                await roleManager.CreateAsync(role);
                logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }

    #endregion

    #region Seed Admin User
    
    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@gmail.com";
        
        if (await userManager.FindByEmailAsync(adminEmail) != null)
        {
            logger.LogInformation("Admin user already exists.");
            return;
        }

        var administrator = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Administrator",
            EmailConfirmed = true,
            PhoneNumber = "+1234567890",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            Created = DateTime.Now,
            CreatedBy = "System"
        };

        var result = await userManager.CreateAsync(administrator, "Ti100600@");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(administrator, nameof(Roles.Admin));
            logger.LogInformation("Created admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    #endregion

    #region Seed Normal Users
    
    private async Task SeedNormalUsersAsync()
    {
        var normalUsers = new List<ApplicationUser>();
        var userNumber = 100;
        for (int i = 1; i <= userNumber; i++)
        {
            normalUsers.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = $"user{i}@gmail.com",
                Email = $"user{i}@gmail.com",
                FirstName = $"User {i}",
                LastName = "System",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = true,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                Created = DateTime.Now,
                CreatedBy = "System"
            });
        }

        foreach (var user in normalUsers)
        {
            if (await userManager.FindByEmailAsync(user.Email) != null)
            {
                logger.LogInformation("User {Email} already exists.", user.Email);
                continue;
            }

            var result = await userManager.CreateAsync(user, "Ti100600@");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, nameof(Roles.User));
                logger.LogInformation("Created user: {Email}", user.Email);
            }
            else
            {
                logger.LogError("Failed to create user {Email}: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    #endregion
}
