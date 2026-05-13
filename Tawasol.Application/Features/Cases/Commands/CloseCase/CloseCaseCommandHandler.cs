using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.CloseCase;

public class CloseCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CloseCaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CloseCaseCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        @case.CloseCase(request.ClosedBy); // Call the domain method to close the case
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true, "Case closed successfully.");
    }
}
