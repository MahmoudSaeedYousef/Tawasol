using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.SubmitFieldReport;

public record SubmitFieldReportCommand(
    Guid CaseId,
    Guid ResearcherId,
    string FieldNotes,
    bool IsUrgent) : IRequest<Result<bool>>;
