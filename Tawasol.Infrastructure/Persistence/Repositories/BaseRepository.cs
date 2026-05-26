using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository<T>(AppDbContext context) where T : class
{
    protected readonly AppDbContext Context = context;

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await Context.Set<T>().FindAsync([id], ct);
    }

    // 🚀 تحديث ذكي: دعم فلترة البيانات اختيارياً عبر الـ Expression
    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<T> query = Context.Set<T>();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        return await query.ToListAsync(ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await Context.Set<T>().AddAsync(entity, ct);
    }

    // الـ EF Core بيعمل Track للتغييرات، فميثود الـ Update مجرد إعلام للسياق
    public virtual void Update(T entity, CancellationToken ct)
    {
        Context.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
    }
}