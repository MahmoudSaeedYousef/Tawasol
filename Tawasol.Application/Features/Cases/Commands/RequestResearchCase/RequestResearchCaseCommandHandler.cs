using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Features.Cases.Commands.ApproveCase;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.RequestResearchCase;

public class RequestResearchCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RequestResearchCaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RequestResearchCaseCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        @case.TransitionTo(CaseStatus.NeedsResearch, actorId: request.UpdatedBy); // Pass actorId
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true, "Case assigned to research successfully.");
    }
}
