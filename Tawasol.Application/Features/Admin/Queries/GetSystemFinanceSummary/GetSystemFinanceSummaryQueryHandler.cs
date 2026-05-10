using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Admin;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Admin.Queries.GetSystemFinanceSummary;

public class GetSystemFinanceSummaryQueryHandler(
    IWalletRepository walletRepository,
    ICaseRepository caseRepository)
    : IRequestHandler<GetSystemFinanceSummaryQuery, Result<FinanceSummaryDto>>
{
    public async Task<Result<FinanceSummaryDto>> Handle(GetSystemFinanceSummaryQuery request, CancellationToken ct)
    {
        var wallets = await walletRepository.GetAllAsync(ct);
        var activeCases = await caseRepository.GetCasesByStatusAsync(CaseStatus.Published, ct);

        var walletDtos = wallets.Select(w => new WalletBalanceDto(w.Category.ToString(), w.Balance)).ToList();
        var summary = new FinanceSummaryDto(
            walletDtos,
            activeCases.Count(),
            activeCases.Sum(c => c.CollectedAmount));

        return Result<FinanceSummaryDto>.Success(summary);
    }
}
