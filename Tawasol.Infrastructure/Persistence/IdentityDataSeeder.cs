using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Tawasol.Domain.Enums;
using Tawasol.Infrastructure.Identity;

namespace Tawasol.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Seed Roles
        var roles = Enum.GetNames<UserRole>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Admin (Hakim)
        const string adminPhone = "000000";
        var adminUser = await userManager.FindByNameAsync(adminPhone);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminPhone,
                PhoneNumber = adminPhone,
                FullName = "System Admin",
                Points = 1000,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(newAdmin, "000000");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, nameof(UserRole.Hakim));
            }
        }
    }
}
