using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Infrastructure.Persistence;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByPhoneAsync(string email, CancellationToken ct = default)
    {
        return await Context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == email, ct);
    }
}
