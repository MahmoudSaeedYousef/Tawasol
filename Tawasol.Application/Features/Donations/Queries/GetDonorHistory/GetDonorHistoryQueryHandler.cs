using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Queries.GetDonorHistory;

public class GetDonorHistoryQueryHandler(
    ITransactionRepository transactionRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetDonorHistoryQuery, Result<IEnumerable<Transaction>>>
{
    public async Task<Result<IEnumerable<Transaction>>> Handle(GetDonorHistoryQuery request, CancellationToken ct)
    {
        var donor = await userRepository.GetByIdAsync(request.DonorId, ct);
        if (donor == null)
            return Result<IEnumerable<Transaction>>.Failure("Donor not found.");

        var transactions = await transactionRepository.GetByDonorIdAsync(request.DonorId, ct);
        return Result<IEnumerable<Transaction>>.Success(transactions);
    }
}
