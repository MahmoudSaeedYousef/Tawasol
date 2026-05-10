using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawasol.Application.Features.Users.Commands.UpdateDeviceToken;
using Tawasol.Application.Features.Users.Queries.GetUserProfile;

namespace Tawasol.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/profile", [Authorize] async (ClaimsPrincipal user, ISender mediator) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await mediator.Send(new GetUserProfileQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetUserProfile");

        group.MapPatch("/device-token", [Authorize] async ([FromBody] string token, ClaimsPrincipal user, ISender mediator) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await mediator.Send(new UpdateDeviceTokenCommand(userId, token));
            return Results.Ok(result);
        })
        .WithName("UpdateDeviceToken");
    }
}
