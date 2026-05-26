using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Application.Features.Cases.Commands.CreateCase;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Domain.ValueObjects;

public class CreateCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileService fileService)
    : IRequestHandler<CreateCaseCommand, Result<CaseResponseDto>>
{
    public async Task<Result<CaseResponseDto>> Handle(CreateCaseCommand request, CancellationToken ct)
    {
        Location? location = null;
        if (request.Latitude != 0 || request.Longitude != 0)
        {
            location = new Location(request.Latitude, request.Longitude);
        }

        var @case = new Case(
                request.Title,
                request.Description,
                0, // CollectedAmount يبدأ بصفر
                request.TargetAmount,
                request.CaseType,
                request.CreatedBy, 
                location,
                request.ExtraDetails);
        
        // 1. إضافة البنود العينية بالكميات الجديدة
        foreach (var item in request.CaseItems)
        {
            // 🚀 التعديل الإستراتيجي 2: تمرير الـ item.TargetAmount (الكمية الإلزامية للـ Domain)
            // تأكد إن ميثود AddItem جوه الـ Case Entity مصلحة لتستقبل الـ int وتمرره للـ Constructor
            @case.AddItem(
                name: item.Name, 
                description: "Item Description", 
                type: request.CaseType, 
                targetAmount: item.TargetAmount, // 👈 باصي الكمية هنا بالملي
                estimatedCost: item.EstimatedCost
            );
        }

        // 2. معالجة وحفظ المرفقات والصور
        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var file in request.Attachments)
            {
                var relativePath = await fileService.SaveFileAsync(file, "cases");

                @case.AddAttachment(
                    fileName: file.FileName,
                    filePath: relativePath, 
                    fileType: file.ContentType
                );
            }
        }
        
        await caseRepository.AddAsync(@case, ct);
        
        // حفظ كل شيء في سياق AppDbContext الموحد والنظيف 💎
        await unitOfWork.SaveChangesAsync(ct);
        
        var response = mapper.Map<CaseResponseDto>(@case);
        return Result<CaseResponseDto>.Success(response);
    }
}