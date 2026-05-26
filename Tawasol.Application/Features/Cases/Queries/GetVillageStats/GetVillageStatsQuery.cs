using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Queries.GetVillageStats
{
    public class GetVillageStatsQuery : IRequest<Result<VillageStatsResponseDto>>
    {
    }
}
