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
    
    public void Update(Case @case)
    {
        context.Cases.Update(@case);
    }
    public override async Task<Case?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Cases
            .Include(c => c.Items)        // 🚀 الحل هنا: إجبار EF Core على تحميل البنود
            .Include(c => c.Attachments)  // 🚀 وتحميل المرفقات بالمرة عشان متضربش بعدين
            .FirstOrDefaultAsync(c => c.Id == id, ct);    }

    public async Task<(int receivedItemCount, int closedCasesCount, decimal totalDonationAmount)> GetVillageStatsAsync( CancellationToken cancellationToken)
    {
        
        var closedCasesCount = await Context.Cases.AsNoTracking().Where(c => c.Status == CaseStatus.Closed)
            .CountAsync(cancellationToken);

        var receivedItemCount= await Context.CaseItems.AsNoTracking().Where(c => c.Status == CaseItemStatus.Delivered)
            .CountAsync(cancellationToken);

        var totalAmount =  await Context.Cases.AsNoTracking()
            .Where(c => c.Status == CaseStatus.Closed && c.CaseType == CaseItemType.Monetary)
            .SumAsync(b => b.CollectedAmount, cancellationToken: cancellationToken);

        
        return (receivedItemCount,closedCasesCount,totalAmount);
    }

    public async Task<(IEnumerable<Case> Cases, int TotalCount)> GetCasesPagedAsync(List<CaseStatus> statuses,
        string? searchTerm,
        string? categoryFilter,
        bool? isUrgent,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = Context.Cases.Include(c => c.Attachments).AsNoTracking().Include(c=>c.Items).AsNoTracking().AsQueryable();

        if (statuses.Count != 0)
        {
            query = query.Where(c => statuses.Contains(c.Status));
        }
        if (isUrgent.HasValue && isUrgent.Value)
        {
            query = query.Where(c =>c.Priority > 0 && c.Priority < 5);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm) || c.Description.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(categoryFilter)&& categoryFilter != "0")
        {
            query = query.Where(c => (int)c.CaseType == int.Parse(categoryFilter));
        }

        var totalCount = await query.CountAsync(ct);
        
        var cases = isUrgent.HasValue && isUrgent.Value ? 
            await query
            .OrderBy(c => c.Priority)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct) 
            : 
            await query
            .OrderByDescending(c =>   c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (cases, totalCount);
    }
}
