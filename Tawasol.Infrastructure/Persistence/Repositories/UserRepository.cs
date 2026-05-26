using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await userManager.FindByIdAsync(id.ToString());
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default)
    {
        // 🚀 استخدام FindByNameAsync أسرع وأضمن لأننا مسيفين الـ Phone جوة الـ UserName
        return await userManager.FindByNameAsync(phone);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainException($"Failed to add user: {errors}"); // يفضل استخدام الـ DomainException بتاعك
        }
    }

    public async Task UpdateAsync(User user)
    {
        // 🚀 تشغيل الـ Async الحقيقي وحماية الـ Threads من الـ Block
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainException($"Failed to update user: {errors}");
        }
    }
}