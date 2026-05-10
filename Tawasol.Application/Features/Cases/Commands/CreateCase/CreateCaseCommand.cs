using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string Title,
    string Description,
    decimal TargetAmount,
    string CaseType,
    Dictionary<string, string> ExtraDetails) : IRequest<Result<CaseResponseDto>>;
