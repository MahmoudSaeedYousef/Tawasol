using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;

namespace Tawasol.Domain.Interfaces.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByCategoryAsync(WalletCategory category, CancellationToken ct = default);
    Task<IEnumerable<Wallet>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Wallet wallet, CancellationToken ct = default);
}
