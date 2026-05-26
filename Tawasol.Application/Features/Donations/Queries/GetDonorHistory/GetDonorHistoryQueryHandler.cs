using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Donations;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Queries.GetDonorHistory;

public class GetDonorHistoryQueryHandler(
    ITransactionRepository transactionRepository,
    IInKindDonationRepository inKindDonationRepository,
    ICaseRepository caseRepository,
    ICaseItemRepository caseItemRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetDonorHistoryQuery, Result<IEnumerable<DonationHistoryDto>>>
{
    public async Task<Result<IEnumerable<DonationHistoryDto>>> Handle(GetDonorHistoryQuery request, CancellationToken ct)
    {
        var donor = await userRepository.GetByIdAsync(request.DonorId, ct);
        if (donor == null)
            return Result<IEnumerable<DonationHistoryDto>>.Failure("Donor not found.");

        var transactions = await transactionRepository.GetByDonorIdAsync(request.DonorId, ct);
        var inKindDonations = await inKindDonationRepository.GetByDonorIdAsync(request.DonorId, ct);

        var historyList = new List<DonationHistoryDto>();

        // Map Financial Transactions
        foreach (var t in transactions)
        {
            string? caseTitle = null;
            if (t.CaseId.HasValue)
            {
                var @case = await caseRepository.GetByIdAsync(t.CaseId.Value, ct);
                caseTitle = @case?.Title;
            }

            historyList.Add(new DonationHistoryDto(
                t.Id,
                "Financial",
                t.Amount,
                t.Status.ToString(),
                t.TransactionDate,
                caseTitle,
                null,
                t.ProofPictureUrl // Include ProofPictureUrl as a fallback, though technically for donors it's their proof, not delivery
            ));
        }

        // Map In-Kind Donations
        foreach (var ik in inKindDonations)
        {
            string? itemName = null;
            string? caseTitle = null;

            var caseItem = await caseItemRepository.GetByIdAsync(ik.CaseItemId, ct);
            if (caseItem != null)
            {
                itemName = caseItem.Name;
                var @case = await caseRepository.GetByIdAsync(caseItem.CaseId, ct);
                caseTitle = @case?.Title;
            }

            historyList.Add(new DonationHistoryDto(
                ik.Id,
                "InKind",
                null,
                ik.Status.ToString(),
                ik.CreatedAt,
                caseTitle,
                itemName,
                ik.DeliveryPhotoUrl // Include DeliveryPhotoUrl as requested
            ));
        }

        // Sort combined list by date descending
        var sortedHistory = historyList.OrderByDescending(h => h.Date).ToList();

        return Result<IEnumerable<DonationHistoryDto>>.Success(sortedHistory);
    }
}
