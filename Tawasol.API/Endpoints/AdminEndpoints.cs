using MediatR;
using Microsoft.AspNetCore.Authorization;
using Tawasol.Application.Features.Admin.Queries.GetSystemFinanceSummary;
using Tawasol.Application.Features.Admin.Queries.GetVillageStats;

namespace Tawasol.API.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/dashboard");

        group.MapGet("/summary", [Authorize(Roles = "Hakim")] async (ISender mediator) =>
        {
            var result = await mediator.Send(new GetSystemFinanceSummaryQuery());
            return Results.Ok(result);
        })
        .WithName("GetSystemFinanceSummary");

        // group.MapGet("/village-stats", async (ISender mediator) =>
        // {
        //     var result = await mediator.Send(new GetVillageStatsQuery());
        //     return Results.Ok(result);
        // })
        // .WithName("GetVillageStats");
    }
}
