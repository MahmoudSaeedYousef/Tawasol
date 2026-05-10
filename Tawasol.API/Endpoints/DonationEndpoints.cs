using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawasol.Application.DTOs.Donations;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;
using Tawasol.Application.Features.Donations.Commands.ConfirmDelivery;
using Tawasol.Application.Features.Donations.Commands.Donate;
using Tawasol.Application.Features.Donations.Commands.DonateToFund;
using Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;
using Tawasol.Application.Features.Donations.Commands.VerifyDonation;
using Tawasol.Application.Features.Donations.Queries.GetDonorHistory;
using Tawasol.Domain.Enums;

namespace Tawasol.API.Endpoints;

public static class DonationEndpoints
{
    public static void MapDonationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/donations");

        // Existing Financial Donation
        group.MapPost("/", [Authorize] async (IFormCollection form, ClaimsPrincipal user, ISender mediator) =>
        {
            var donorId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var amount = decimal.Parse(form["amount"]!);
            var caseIdStr = form["caseId"].ToString();
            Guid? caseId = string.IsNullOrEmpty(caseIdStr) ? null : Guid.Parse(caseIdStr);
            
            var file = form.Files.GetFile("proofImage");
            if (file == null) return Results.BadRequest("Proof image is required.");

            var fileModel = new FileModel(file.OpenReadStream(), file.FileName, file.ContentType);
            var command = new DonateCommand(donorId, amount, caseId, fileModel);
            
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .DisableAntiforgery();

        // In-Kind Pledge
        group.MapPost("/pledge", [Authorize] async ([FromBody] PledgeRequest request, ClaimsPrincipal user, ISender mediator) =>
        {
            var donorId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new PledgeInKindDonationCommand(donorId, request.CaseId, request.CaseItemId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("PledgeInKind");

        // Donate to General Fund
        group.MapPost("/fund", [Authorize] async (IFormCollection form, ClaimsPrincipal user, ISender mediator) =>
        {
            var donorId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var amount = decimal.Parse(form["amount"]!);
            var category = Enum.Parse<WalletCategory>(form["category"]!);
            
            var file = form.Files.GetFile("proofImage");
            if (file == null) return Results.BadRequest("Proof image is required.");

            var fileModel = new FileModel(file.OpenReadStream(), file.FileName, file.ContentType);
            var command = new DonateToFundCommand(donorId, amount, category, fileModel);
            
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithName("DonateToFund");

        // Confirm Delivery (Hakim/Researcher)
        group.MapPatch("/{id:guid}/confirm-delivery", [Authorize(Roles = "Hakim,Researcher")] async (Guid id, IFormFile proofFile, ISender mediator) =>
        {
            var fileModel = new FileModel(proofFile.OpenReadStream(), proofFile.FileName, proofFile.ContentType);
            var result = await mediator.Send(new ConfirmDeliveryCommand(id, fileModel));
            return Results.Ok(result);
        })
        .DisableAntiforgery()
        .WithName("ConfirmDelivery");

        group.MapPatch("/{id:guid}/verify", [Authorize(Roles = "Hakim")] async (Guid id, ISender mediator) =>
        {
            var result = await mediator.Send(new VerifyDonationCommand(id));
            return Results.Ok(result);
        })
        .WithName("VerifyDonation");

        group.MapGet("/my-history", [Authorize] async (ClaimsPrincipal user, ISender mediator) =>
        {
            var donorId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await mediator.Send(new GetDonorHistoryQuery(donorId));
            return Results.Ok(result);
        })
        .WithName("GetMyDonationHistory");
    }
}

