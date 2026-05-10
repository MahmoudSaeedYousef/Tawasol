using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class WalletRepository(AppDbContext context) : BaseRepository<Wallet>(context), IWalletRepository
{
    public async Task<Wallet?> GetByCategoryAsync(WalletCategory category, CancellationToken ct = default)
    {
        return await Context.Set<Wallet>().FirstOrDefaultAsync(w => w.Category == category, ct);
    }

    public async Task<IEnumerable<Wallet>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.Set<Wallet>().ToListAsync(ct);
    }
}
