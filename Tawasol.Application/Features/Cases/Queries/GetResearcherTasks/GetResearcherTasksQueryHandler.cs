using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Queries.GetResearcherTasks;

public class GetResearcherTasksQueryHandler(
    ICaseRepository caseRepository,
    IMapper mapper)
    : IRequestHandler<GetResearcherTasksQuery, Result<IEnumerable<CaseResponseDto>>>
{
    public async Task<Result<IEnumerable<CaseResponseDto>>> Handle(GetResearcherTasksQuery request, CancellationToken ct)
    {
        var cases = await caseRepository.GetCasesByStatusAsync(CaseStatus.Pending, ct); // Assuming this method exists or will be added
        var response = mapper.Map<IEnumerable<CaseResponseDto>>(cases);
        return Result<IEnumerable<CaseResponseDto>>.Success(response);
    }
}
