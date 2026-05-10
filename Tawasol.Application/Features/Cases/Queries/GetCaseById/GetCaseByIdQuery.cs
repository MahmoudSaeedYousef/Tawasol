using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;

namespace Tawasol.Application.Features.Cases.Queries.GetCaseById;

public record GetCaseByIdQuery(Guid Id) : IRequest<Result<CaseResponseDto>>;
