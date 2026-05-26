using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public class CaseItemRepository(AppDbContext context) : ICaseItemRepository
{
    public async Task<CaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.CaseItems.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<CaseItem>> GetAllAsync(Expression<Func<CaseItem, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = context.CaseItems.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }
    public async Task<List<CaseItem>> GetAllByCaseIdAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        return await context.CaseItems
            .Where(ci => ci.CaseId == caseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CaseItem>> GetByStatusAsync(CaseItemStatus status, CancellationToken ct = default)
    {
        return await context.CaseItems
            .Where(ci => ci.Status == status)
            .ToListAsync(ct);
    }

    public async Task<Case?> GetCaseByItemIdAsync(Guid itemId, CancellationToken ct = default)
    {
        var item = await context.CaseItems.FindAsync(new object[] { itemId }, ct);
        if (item == null) return null;
        return await context.Cases.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == item.CaseId, ct);
    }

    public void Update(CaseItem caseItem)
    {
        context.CaseItems.Update(caseItem);
    }
}
