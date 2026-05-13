using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.RejectCase;

public record RejectCaseCommand(Guid CaseId, string Reason, Guid RejectedBy) : IRequest<Result<bool>>;
