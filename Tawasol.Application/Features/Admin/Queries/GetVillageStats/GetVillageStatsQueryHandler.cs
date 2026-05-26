using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Admin.Queries.GetVillageStats;

public class GetVillageStatsQueryHandler(ICaseItemRepository caseItemRepository)
    : IRequestHandler<GetVillageStatsQuery, Result<Dictionary<string, int>>>
{
    public async Task<Result<Dictionary<string, int>>> Handle(GetVillageStatsQuery request, CancellationToken ct)
    {
        var deliveredItems = await caseItemRepository.GetByStatusAsync(CaseItemStatus.Delivered, ct);

        // Group by item name and count them
        // Note: Grouping by 'Name' assumes names are consistent (e.g., "Fridge", "Blanket").
        var stats = deliveredItems
            .GroupBy(i => i.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        return Result<Dictionary<string, int>>.Success(stats);
    }
}
