using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Commands.Donate;

public class DonateCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    IFileService fileService)
    : IRequestHandler<DonateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DonateCommand request, CancellationToken ct)
    {
        var proofUrl = await fileService.SaveFileAsync(request.ProofImage.Stream, request.ProofImage.FileName, "donations", ct);
        
        var transaction = new Transaction(request.DonorId, request.Amount, proofUrl, request.CaseId);
        
        await transactionRepository.AddAsync(transaction, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<Guid>.Success(transaction.Id, "Donation submitted for verification.");
    }
}
