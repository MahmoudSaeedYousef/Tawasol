using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.RejectCase;

public class RejectCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RejectCaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RejectCaseCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        @case.TransitionTo(CaseStatus.Rejected, rejectionReason: request.Reason);
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true, "Case rejected successfully.");
    }
}
