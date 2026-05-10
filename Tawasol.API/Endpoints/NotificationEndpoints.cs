using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Tawasol.Application.Features.Notifications.Queries.GetMyNotifications;

namespace Tawasol.API.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications");

        group.MapGet("/", [Authorize] async (ClaimsPrincipal user, ISender mediator) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await mediator.Send(new GetMyNotificationsQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetMyNotifications");
    }
}
