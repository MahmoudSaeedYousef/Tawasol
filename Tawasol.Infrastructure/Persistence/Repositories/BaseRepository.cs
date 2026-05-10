using Microsoft.EntityFrameworkCore;
using Tawasol.Infrastructure.Persistence;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T>(AppDbContext context)
    where T : class
{
    protected readonly AppDbContext Context = context;

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Context.Set<T>().FindAsync([id], ct);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await Context.Set<T>().ToListAsync(ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await Context.Set<T>().AddAsync(entity, ct);
    }

    public virtual void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
    }
}
