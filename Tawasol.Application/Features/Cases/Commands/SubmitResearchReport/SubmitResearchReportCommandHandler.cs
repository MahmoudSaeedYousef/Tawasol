using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.SubmitResearchReport;

public class SubmitResearchReportCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitResearchReportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SubmitResearchReportCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        @case.SubmitResearch(request.ResearcherId, request.FieldNotes, request.IsVerified);
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true, "Research report submitted successfully.");
    }
}
