using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.SubmitFieldReport;

public class SubmitFieldReportCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitFieldReportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SubmitFieldReportCommand request, CancellationToken ct)
    {
        try
        {
            var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
            if (@case == null)
                return Result<bool>.Failure("Case not found.");

            var report = new VerificationReport(request.CaseId, request.ResearcherId, request.FieldNotes, request.IsUrgent);
            
            // The domain method TransitionTo handles status and report assignment
            @case.TransitionTo(CaseStatus.Researched, report: report);
            
            await unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true, "Field report submitted successfully.");
        }
        catch (DomainException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
