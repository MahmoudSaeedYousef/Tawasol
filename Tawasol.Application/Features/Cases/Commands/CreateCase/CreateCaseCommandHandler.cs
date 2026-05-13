using AutoMapper;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Cases;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.CreateCase;
public class CreateCaseCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileService fileService) // 👈 حقن خدمة الملفات
    : IRequestHandler<CreateCaseCommand, Result<CaseResponseDto>>
{
    public async Task<Result<CaseResponseDto>> Handle(CreateCaseCommand request, CancellationToken ct)
    {
        var @case = new Case(
            request.Title, 
            request.Description, 
            request.TargetAmount, 
            request.CaseType, 
            request.CreatedBy, 
            request.ExtraDetails);
        
        // 1. إضافة البنود
        foreach (var item in request.CaseItems)
        {
            @case.AddItem(item.Name, "Item Description", request.CaseType, item.EstimatedCost);
        }

        // 2. معالجة وحفظ الصور 🚀
        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var file in request.Attachments)
            {
                // حفظ الملف في المسار الفيزيائي
                var relativePath = await fileService.SaveFileAsync(file, "cases");

                // تسجيل المرفق في الـ Domain Entity
                @case.AddAttachment(
                    fileName: file.FileName,
                    filePath: relativePath, 
                    fileType: file.ContentType
                );
            }
        }
        
        await caseRepository.AddAsync(@case, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var response = mapper.Map<CaseResponseDto>(@case);
        return Result<CaseResponseDto>.Success(response);
    }
}