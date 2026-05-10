using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;

namespace Tawasol.Domain.Interfaces.Repositories;

public interface ICaseRepository
{
    Task<Case?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Case>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Case>> GetCasesByStatusAsync(CaseStatus status, CancellationToken ct = default);
    Task AddAsync(Case @case, CancellationToken ct = default);
    void Update(Case @case);
    void Delete(Case @case);
}
