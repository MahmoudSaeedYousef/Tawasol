using Tawasol.Domain.Entities;

namespace Tawasol.Domain.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetByCaseIdAsync(Guid caseId, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetByDonorIdAsync(Guid donorId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
}
