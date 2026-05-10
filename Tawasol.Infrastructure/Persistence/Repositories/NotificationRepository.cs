using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext context) : BaseRepository<Notification>(context), INotificationRepository
{
    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.Set<Notification>()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
    }
}
