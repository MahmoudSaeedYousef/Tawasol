using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Exceptions;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Queries.GetCaseById;

public class GetCaseByIdQueryHandler(ICaseRepository caseRepository, IMapper mapper)
    : IRequestHandler<GetCaseByIdQuery, Result<CaseResponseDto>>
{
    public async Task<Result<CaseResponseDto>> Handle(GetCaseByIdQuery request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.Id, ct);
        
        if (@case == null)
            throw new NotFoundException("Case", request.Id);
            
        var response = mapper.Map<CaseResponseDto>(@case);
        return Result<CaseResponseDto>.Success(response);
    }
}
