using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;
using Tawasol.Application.Features.Cases.Commands.ApproveCase;
using Tawasol.Application.Features.Cases.Commands.CreateCase;
using Tawasol.Application.Features.Cases.Commands.RejectCase;
using Tawasol.Application.Features.Cases.Commands.SubmitFieldReport;
using Tawasol.Application.Features.Cases.Queries.GetCaseById;
using Tawasol.Application.Features.Cases.Queries.GetResearcherTasks;

namespace Tawasol.API.Endpoints;

public static class CaseEndpoints
{
    public static void MapCaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cases");

        group.MapPost("/", [Authorize(Roles = "GeneralUser")] async (CreateCaseCommand command, ISender mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateCase");

        group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
        {
            var result = await mediator.Send(new GetCaseByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetCaseById");

        group.MapPost("/{id:guid}/attachments", [Authorize] async (Guid id, IFormFileCollection files, ISender mediator) =>
        {
            var fileModels = files.Select(f => new FileModel(f.OpenReadStream(), f.FileName, f.ContentType)).ToList();
            var result = await mediator.Send(new AddCaseAttachmentsCommand(id, fileModels));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithName("AddCaseAttachments");

        // Researcher Endpoints
        group.MapGet("/tasks", [Authorize(Roles = "Researcher")] async (ISender mediator) =>
        {
            var result = await mediator.Send(new GetResearcherTasksQuery());
            return Results.Ok(result);
        })
        .WithName("GetResearcherTasks");

        group.MapPost("/{id:guid}/report", [Authorize(Roles = "Researcher")] async (Guid id, [FromBody] SubmitFieldReportRequestDto request, ClaimsPrincipal user, ISender mediator) =>
        {
            var researcherId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new SubmitFieldReportCommand(id, researcherId, request.FieldNotes, request.IsUrgent);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("SubmitFieldReport");

        // Hakim Endpoints
        group.MapPatch("/{id:guid}/approve", [Authorize(Roles = "Hakim")] async (Guid id, ISender mediator) =>
        {
            var result = await mediator.Send(new ApproveCaseCommand(id));
            return Results.Ok(result);
        })
        .WithName("ApproveCase");

        group.MapPatch("/{id:guid}/reject", [Authorize(Roles = "Hakim")] async (Guid id, [FromBody] RejectCaseRequestDto request, ISender mediator) =>
        {
            var result = await mediator.Send(new RejectCaseCommand(id, request.Reason));
            return Results.Ok(result);
        })
        .WithName("RejectCase");
    }
}
