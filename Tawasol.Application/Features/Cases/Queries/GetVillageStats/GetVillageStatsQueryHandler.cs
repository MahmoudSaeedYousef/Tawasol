using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Queries.GetVillageStats
{
    public class GetVillageStatsQueryHandler(ICaseRepository caseRepository)
        : IRequestHandler<GetVillageStatsQuery, Result<VillageStatsResponseDto>>
    {
        public async Task<Result<VillageStatsResponseDto>> Handle(GetVillageStatsQuery request, CancellationToken cancellationToken)
        {
            var deliveredItems = await caseRepository.GetVillageStatsAsync(cancellationToken: cancellationToken);

            var stats = new VillageStatsResponseDto
            {
                TotalDeliveredItems = deliveredItems.receivedItemCount,
                TotalClosedCases = deliveredItems.closedCasesCount,
                TotalDonationAmount = deliveredItems.totalDonationAmount
            };

            return Result<VillageStatsResponseDto>.Success(stats);
        }
    }
}
