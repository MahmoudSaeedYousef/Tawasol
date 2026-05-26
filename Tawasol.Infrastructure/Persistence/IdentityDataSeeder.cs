using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums; // تأكد من مسار الـ Namespace للـ User والـ UserRole

namespace Tawasol.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        // 🚀 التحديث السحري: جلب الـ UserManager الخاص بالـ User Domain Entity
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        // استخدام IdentityRole<Guid> لأن الـ User شغال بـ Guid
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // 1. Seed Roles
        var roles = Enum.GetNames<UserRole>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // 2. Seed Admin (Hakim)
        const string adminPhone = "000000";
        var adminUser = await userManager.FindByNameAsync(adminPhone);

        if (adminUser == null)
        {
            // 🚀 احترام الـ Domain Rules: بناء الـ Admin عن طريق الـ Constructor النظيف بتاعه
            var newAdmin = new User(
                fullName: "System Admin",
                phoneNumber: adminPhone,
                role: UserRole.Hakim
            );

            // بما إن الـ Points محروقة بـ private set وبتبدأ بـ 0، هنرفعها للـ Admin عن طريق ميثود الـ Domain
            newAdmin.AddPoints(1000); 
            newAdmin.PhoneNumberConfirmed = true; // خاصية موروثه من مايكروسوفت نرفعها عادي

            // كريت الـ User جوه جداول الـ Identity مع الباسورد
            var result = await userManager.CreateAsync(newAdmin, "000000");

            if (result.Succeeded)
            {
                // ربط الحكيم بالـ Role بتاعته رسمياً
                await userManager.AddToRoleAsync(newAdmin, nameof(UserRole.Hakim));
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed Admin User: {errors}");
            }
        }
    }
}