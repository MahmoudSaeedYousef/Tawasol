using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;

namespace Tawasol.Domain.Interfaces.Repositories;

public interface ICaseItemRepository
{
    Task<CaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<CaseItem>> GetAllAsync(Expression<Func<CaseItem, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<List<CaseItem>> GetAllByCaseIdAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CaseItem>> GetByStatusAsync(CaseItemStatus status, CancellationToken ct = default);
    Task<Case?> GetCaseByItemIdAsync(Guid itemId, CancellationToken ct = default);
    void Update(CaseItem caseItem);
}
