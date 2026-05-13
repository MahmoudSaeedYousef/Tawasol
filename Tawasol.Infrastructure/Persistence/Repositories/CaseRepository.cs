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

    public async Task<(IEnumerable<Case> Cases, int TotalCount)> GetCasesPagedAsync(
        List<CaseStatus> statuses, 
        string? searchTerm, 
        string? categoryFilter, 
        int pageNumber, 
        int pageSize, 
        CancellationToken ct = default)
    {
        var query = Context.Cases.Include(c => c.Attachments).AsQueryable();

        if (statuses != null && statuses.Any())
        {
            query = query.Where(c => statuses.Contains(c.Status));
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
        
        var cases = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (cases, totalCount);
    }
}
