using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Commands.VerifyDonation;

public class VerifyDonationCommandHandler(
    ITransactionRepository transactionRepository,
    ICaseRepository caseRepository,
    IWalletRepository walletRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFcmService fcmService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyDonationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(VerifyDonationCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var transaction = await transactionRepository.GetByIdAsync(request.TransactionId, ct);
            if (transaction == null)
                return Result<bool>.Failure("Transaction not found.");

            transaction.Verify();

            // 1. Update Case if applicable
            if (transaction.CaseId.HasValue)
            {
                var @case = await caseRepository.GetByIdAsync(transaction.CaseId.Value, ct);
                if (@case != null)
                {
                    @case.AddContribution(transaction.Amount);
                }
            }

            // 2. Update Wallet
            var category = transaction.CaseId.HasValue ? WalletCategory.CasesFund : WalletCategory.GeneralFund;
            var wallet = await walletRepository.GetByCategoryAsync(category, ct);
            
            if (wallet == null)
            {
                wallet = new Domain.Entities.Wallet(category);
                await walletRepository.AddAsync(wallet, ct);
            }
            
            wallet.Deposit(transaction.Amount);

            // 3. Add Points to Donor (1 point per 10 EGP)
            var donor = await userRepository.GetByIdAsync(transaction.DonorId, ct);
            if (donor != null)
            {
                int points = (int)(transaction.Amount / 10);
                donor.AddPoints(points);

                // 4. Trigger Notification
                var title = "Donation Verified!";
                var message = $"Your donation of {transaction.Amount} EGP has been verified. Impact points added!";
                
                var notification = new Notification(donor.Id, title, message);
                await notificationRepository.AddAsync(notification, ct);

                if (!string.IsNullOrEmpty(donor.DeviceToken))
                {
                    await fcmService.SendNotificationAsync(donor.DeviceToken, title, message);
                }
            }

            await unitOfWork.CommitTransactionAsync(ct);
            return Result<bool>.Success(true, "Donation verified and funds allocated.");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            return Result<bool>.Failure(ex.Message);
        }
    }
}
