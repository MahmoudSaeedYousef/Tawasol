using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Admin.Queries.GetVillageStats;

public record GetVillageStatsQuery() : IRequest<Result<Dictionary<string, int>>>;
