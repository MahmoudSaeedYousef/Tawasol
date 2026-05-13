using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;

public class AddCaseAttachmentsCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork,
    IFileService fileService)
    : IRequestHandler<AddCaseAttachmentsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddCaseAttachmentsCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null)
            return Result<bool>.Failure("Case not found.");

        foreach (var file in request.Files)
        {
            
            var filePath = await fileService.SaveFileAsync(file.Stream, file.FileName, "cases", ct);
            
            // Log for debugging
            Console.WriteLine($">>> Saving attachment for Case: {@case.Id}");
            
            @case.AddAttachment(file.FileName, filePath, file.ContentType);
        }

        try 
        {
            await unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true, "Attachments added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> SAVE ERROR: {ex.Message}");
            throw;
        }
    }
}
