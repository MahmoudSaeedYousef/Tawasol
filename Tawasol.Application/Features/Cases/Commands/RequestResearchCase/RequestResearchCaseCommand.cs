using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.RequestResearchCase;

public record RequestResearchCaseCommand(Guid CaseId, Guid UpdatedBy) : IRequest<Result<bool>>;
