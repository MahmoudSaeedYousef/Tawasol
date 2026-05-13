using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;
using Tawasol.Application.Features.Cases.Commands.ApproveCase;
using Tawasol.Application.Features.Cases.Commands.CloseCase;
using Tawasol.Application.Features.Cases.Commands.CreateCase;
using Tawasol.Application.Features.Cases.Commands.RejectCase;
using Tawasol.Application.Features.Cases.Commands.SubmitFieldReport;
using Tawasol.Application.Features.Cases.Queries.GetCaseById;
using Tawasol.Application.Features.Cases.Queries.GetCases;
using Tawasol.Application.Features.Cases.Queries.GetResearcherTasks;
using Tawasol.Domain.Enums;

namespace Tawasol.API.Endpoints;

public static class CaseEndpoints
{
    public static void MapCaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cases");

        // Get all cases with optional status filter and pagination
        group.MapGet("/", async (
                [FromQuery] string? statuses,
                [FromQuery] string? searchTerm,
                [FromQuery] string? categoryFilter,
                ISender mediator,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10) =>
            {
                var caseStatuses = new List<CaseStatus>();
                if (!string.IsNullOrEmpty(statuses))
                {
                    var statusList = statuses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var status in statusList)
                    {
                        if (Enum.TryParse(status, true, out CaseStatus parsedStatus))
                        {
                            caseStatuses.Add(parsedStatus);
                        }
                    }
                }
                var query = new GetCasesQuery(caseStatuses,searchTerm, categoryFilter, new PaginationParams(pageNumber, pageSize));
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCases");

        group.MapPost("/", async (HttpRequest request, [FromForm] CreateCaseCommand command, ClaimsPrincipal user, ISender mediator) =>
            {
                // 1. التأكد من الهوية كالعادة
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim)) return Results.Unauthorized();

                // 2. 🚀 الحل السحري: اسحب كل الملفات المرفقة في الـ Request وحطها في الـ Command
                // الطريقة دي بتضمن إن أي ملف مبعوث يوصل، بقطع النظر عن الـ Key Name في فلاتر
                if (request.HasFormContentType)
                {
                    command.Attachments = request.Form.Files.ToList();
                }

                // 3. حقن الـ UserId
                command.CreatedBy = Guid.Parse(userIdClaim);

                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(policy => policy.RequireRole("GeneralUser"))
            .DisableAntiforgery()
            .WithName("CreateCase");

        group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var result = await mediator.Send(new GetCaseByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetCaseById");

        group.MapPost("/{id:guid}/attachments", [Authorize]
                async (Guid id, IFormFileCollection files, ISender mediator) =>
                {
                    var fileModels = files.Select(f => new FileModel(f.OpenReadStream(), f.FileName, f.ContentType))
                        .ToList();
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

        group.MapPost("/{id:guid}/report", [Authorize(Roles = "Researcher")]
                async (Guid id, [FromBody] SubmitFieldReportRequestDto request, ClaimsPrincipal user,
                    ISender mediator) =>
                {
                    var researcherId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var command = new SubmitFieldReportCommand(id, researcherId, request.FieldNotes, request.IsUrgent);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
            .WithName("SubmitFieldReport");

        // Hakim Endpoints
        group.MapPatch("/{id:guid}/approve", [Authorize(Roles = "Hakim")]
                async (Guid id, ClaimsPrincipal user, ISender mediator) =>
                {
                    var approvedBy = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var result = await mediator.Send(new ApproveCaseCommand(id, approvedBy));
                    return Results.Ok(result);
                })
            .WithName("ApproveCase");

        group.MapPatch("/{id:guid}/reject", [Authorize(Roles = "Hakim")]
                async (Guid id, [FromBody] RejectCaseRequestDto request, ClaimsPrincipal user, ISender mediator) =>
                {
                    var rejectedBy = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var result = await mediator.Send(new RejectCaseCommand(id, request.Reason, rejectedBy));
                    return Results.Ok(result);
                })
            .WithName("RejectCase");

        group.MapPatch("/{id:guid}/close", [Authorize(Roles = "Hakim")]
                async (Guid id, ClaimsPrincipal user, ISender mediator) =>
                {
                    var closedBy = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var result = await mediator.Send(new CloseCaseCommand(id, closedBy));
                    return Results.Ok(result);
                })
            .WithName("CloseCase");
    }
}
