using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Cases.Commands.ApproveCase;

public record ApproveCaseCommand(Guid CaseId) : IRequest<Result<bool>>;
