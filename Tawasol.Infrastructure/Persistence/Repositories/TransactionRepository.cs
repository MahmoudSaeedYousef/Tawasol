using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : BaseRepository<Transaction>(context), ITransactionRepository
{
    public async Task<IEnumerable<Transaction>> GetByCaseIdAsync(Guid caseId, CancellationToken ct = default)
    {
        return await Context.Set<Transaction>()
            .Where(t => t.CaseId == caseId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Transaction>> GetByDonorIdAsync(Guid donorId, CancellationToken ct = default)
    {
        return await Context.Set<Transaction>()
            .Where(t => t.DonorId == donorId)
            .ToListAsync(ct);
    }
}
