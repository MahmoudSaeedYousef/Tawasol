using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<CreateCaseCommand, Result<CaseResponseDto>>
{
    public async Task<Result<CaseResponseDto>> Handle(CreateCaseCommand request, CancellationToken ct)
    {
        var @case = new Case(
            request.Title, 
            request.Description, 
            request.TargetAmount, 
            request.CaseType, 
            request.ExtraDetails);
        
        await caseRepository.AddAsync(@case, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var response = mapper.Map<CaseResponseDto>(@case);
        return Result<CaseResponseDto>.Success(response);
    }
}
