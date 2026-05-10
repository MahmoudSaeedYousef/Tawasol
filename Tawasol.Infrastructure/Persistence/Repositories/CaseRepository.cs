using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Infrastructure.Persistence;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class CaseRepository(AppDbContext context) : BaseRepository<Case>(context), ICaseRepository
{
    public async Task<IEnumerable<Case>> GetCasesByStatusAsync(CaseStatus status, CancellationToken ct = default)
    {
        return await Context.Cases
            .Where(c => c.Status == status)
            .ToListAsync(ct);
    }
}
