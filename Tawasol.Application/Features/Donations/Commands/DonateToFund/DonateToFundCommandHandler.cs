using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Commands.DonateToFund;

public class DonateToFundCommandHandler(
    IWalletRepository walletRepository,
    IFileService fileService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DonateToFundCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DonateToFundCommand request, CancellationToken ct)
    {
        var wallet = await walletRepository.GetByCategoryAsync(request.Category, ct);
        if (wallet == null)
        {
            wallet = new Wallet(request.Category);
            await walletRepository.AddAsync(wallet, ct);
        }

        var proofUrl = await fileService.SaveFileAsync(request.ProofImage.Stream, request.ProofImage.FileName, "donations", ct);
        
        var donation = Donation.CreateFinancial(request.DonorId, request.Amount, proofUrl, walletId: wallet.Id);
        
        // At verification time, the wallet balance will be updated.
        // But the requirement says "Update balance" here or in Verify?
        // Usually, verification updates balance. Let's keep consistency with the Verify logic.
        await walletRepository.AddAsync(wallet, ct); // 👈 هنا الـ EF Core بدأ يعمل Tracking للـ Wallet
        await unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Success(donation.Id, "Fund donation submitted for verification.");
    }
}
