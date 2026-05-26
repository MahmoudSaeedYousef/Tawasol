using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.BackgroundJobs;

public class PledgeExpirationJob(
    ICaseItemRepository caseItemRepository,
    IInKindDonationRepository inKindDonationRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFcmService fcmService,
    IUnitOfWork unitOfWork,
    ILogger<PledgeExpirationJob> logger) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation(">>> Running PledgeExpirationJob (Quantity-Aware)...");

        // 1. 🚀 جلب كل حركات التبرع العيني الملعقة (Pending) من الـ Repository
        var pendingDonations = await inKindDonationRepository.GetAllAsync(d => d.Status == DonationStatus.Pending);
        
        var cancellationThreshold = DateTime.UtcNow.AddHours(-48);
        var reminderThreshold = DateTime.UtcNow.AddHours(-24);

        foreach (var donation in pendingDonations)
        {
            // الاعتماد على وقت إنشاء التبرع العيني نفسه (كل متبرع مستقل بوقت تعهده)
            if (donation.CreatedAt <= cancellationThreshold)
            {
                await ProcessExpiredDonationAsync(donation);
            }
            else if (donation.CreatedAt <= reminderThreshold)
            {
                // Send reminder notification (يمكنك تفعيل لوجيك الإشعار هنا لاحقاً)
                logger.LogInformation($"Donation {donation.Id} by Donor {donation.DonorId} is over 24 hours old. Reminder should be sent.");
            }
        }
    }

    private async Task ProcessExpiredDonationAsync(InKindDonation donation)
    {
        // استخدام الـ CancellationToken الافتراضي للـ Background Worker
        var ct = CancellationToken.None;
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var item = await caseItemRepository.GetByIdAsync(donation.CaseItemId, ct);
            if (item == null)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                logger.LogWarning($"Case item {donation.CaseItemId} not found for expired donation {donation.Id}");
                return;
            }

            // 🚀 التعديل 2: تراجع عن الكمية المحددة الخاصة بالمتبرع ده فقط داخل الـ Domain Entity
            item.RevertPledge(donation.Quantity);
            caseItemRepository.Update(item);

            // 🚀 التعديل 3: إلغاء/رفض حركة التبرع العيني دي بالتحديد وتحويلها لـ Cancelled أو Rejected
            donation.Reject(); // أو donation.Cancel() حسب الميثود عندك
            inKindDonationRepository.Update(donation);

            // 🚀 التعديل 4: إشعار المتبرع المتأخر بأدب
            var donor = await userRepository.GetByIdAsync(donation.DonorId, ct);
            if (donor != null)
            {
                var title = "تحديث حالة التعهد";
                var message = $"التعهد الخاص بك لـ ({item.Name}) بعدد ({donation.Quantity}) تم إلغاؤه تلقائياً لعدم التسليم خلال 48 ساعة. نرجو أن تكون بخير.";

                // خط الدفاع المعماري لكسر لغم الـ TKey Compilation
                Guid donorId = donor.Id;

                var notification = new Notification(donorId, title, message);
                await notificationRepository.AddAsync(notification, ct);

                if (!string.IsNullOrEmpty(donor.DeviceToken))
                {
                    await fcmService.SendNotificationAsync(donor.DeviceToken, title, message);
                }
            }

            await unitOfWork.CommitTransactionAsync(ct);
            logger.LogInformation($"Successfully expired and reverted {donation.Quantity} units for donation {donation.Id}");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            logger.LogError(ex, $"Failed to process expired donation {donation.Id}");
        }
    }
}