using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.CloseCase;

public record CloseCaseCommand(Guid CaseId, Guid ClosedBy) : IRequest<Result<bool>>;
