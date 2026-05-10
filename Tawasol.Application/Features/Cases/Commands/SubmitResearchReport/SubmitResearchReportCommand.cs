using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.SubmitResearchReport;

public record SubmitResearchReportCommand(
    Guid CaseId,
    Guid ResearcherId,
    string FieldNotes,
    bool IsVerified) : IRequest<Result<bool>>;
