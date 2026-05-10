using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawasol.Application.Features.Auth.Commands.Login;
using Tawasol.Application.Features.Auth.Commands.Register;

namespace Tawasol.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async ([FromBody] RegisterCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("Register");

        group.MapPost("/login", async ([FromBody] LoginCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("Login");
    }
}
