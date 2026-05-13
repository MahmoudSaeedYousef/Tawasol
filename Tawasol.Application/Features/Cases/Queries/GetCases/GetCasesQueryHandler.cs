using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Queries.GetCases;

public class GetCasesQueryHandler(
    ICaseRepository caseRepository,
    IMapper mapper)
    : IRequestHandler<GetCasesQuery, PagedResult<CaseResponseDto>>
{
    public async Task<PagedResult<CaseResponseDto>> Handle(GetCasesQuery request, CancellationToken ct)
    {
        // If no statuses are provided, default to Published cases
        var statusesToQuery = request.Statuses != null && request.Statuses.Any() 
            ? request.Statuses 
            : new List<CaseStatus> { CaseStatus.Published };

        var result = await caseRepository.GetCasesPagedAsync(
            statusesToQuery,
            request.SearchTerm,
            request.CategoryFilter,
            request.Pagination.PageNumber,
            request.Pagination.PageSize,
            ct);

        var caseDtos = mapper.Map<IEnumerable<CaseResponseDto>>(result.Cases);
        
        return PagedResult<CaseResponseDto>.Success(
            caseDtos,
            result.TotalCount,
            request.Pagination.PageNumber,
            request.Pagination.PageSize);
    }
}
