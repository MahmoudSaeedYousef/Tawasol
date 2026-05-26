using Tawasol.Domain.Entities;

namespace Tawasol.Domain.Interfaces.Repositories; // أو الـ Namespace بتاعك في الـ Application

public interface IInKindDonationRepository
{
    Task<InKindDonation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<InKindDonation>> GetByDonorIdAsync(Guid donorId, CancellationToken ct = default);
    Task<IEnumerable<InKindDonation>> GetByCaseItemIdAsync(Guid caseItemId, CancellationToken ct = default);
    Task<IEnumerable<InKindDonation>> GetAllAsync(System.Linq.Expressions.Expression<Func<InKindDonation, bool>>? predicate = null, CancellationToken ct = default);
    Task AddAsync(InKindDonation donation, CancellationToken ct = default);
    void Update(InKindDonation donation, CancellationToken ct = default);
}