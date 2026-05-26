using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Cases.Queries.GetCases;

public record GetCasesQuery(
    List<CaseStatus> Statuses,
    string SearchTerm,
    string CategoryFilter,
    bool? isUrgent,
    PaginationParams Pagination) : IRequest<PagedResult<CaseResponseDto>>;
