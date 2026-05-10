using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;

namespace Tawasol.Application.Features.Cases.Queries.GetResearcherTasks;

public record GetResearcherTasksQuery() : IRequest<Result<IEnumerable<CaseResponseDto>>>;
