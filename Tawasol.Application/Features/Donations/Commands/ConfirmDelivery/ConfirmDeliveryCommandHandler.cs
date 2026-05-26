using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Enums;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;
using Tawasol.Domain.ValueObjects;

namespace Tawasol.Application.Features.Donations.Commands.ConfirmDelivery;


public class ConfirmDeliveryCommandHandler(
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ICaseItemRepository caseItemRepository,
    ICaseRepository caseRepository,
    IInKindDonationRepository inKindDonationRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IFcmService fcmService,
    ICaseUpdateService caseUpdateService)
    : IRequestHandler<ConfirmDeliveryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ConfirmDeliveryCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var item = await caseItemRepository.GetByIdAsync(request.CaseItemId, ct);
            if (item == null)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                return Result<bool>.Failure("Case item not found.");
            }

            // 🚀 التعديل 2: تغيير شرط الحالة القديم؛ التحقق من أن هناك كميات متعهد بها تكفي للتسليم
            if (item.FulfilledAmount + request.DeliveredQuantity > item.PledgedAmount)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                return Result<bool>.Failure($"الكمية المراد تسليمها أكبر من الكمية المتعهد بها حالياً. المتعهد به المتبقي: {item.PledgedAmount - item.FulfilledAmount}");
            }

            var @case = await caseRepository.GetByIdAsync(item.CaseId, ct);
            if (@case == null)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                return Result<bool>.Failure("Case not found.");
            }

            // Location Verification (GPS Match)
            if (@case.Location != null)
            {
                var deliveryLocation = new Location(request.Latitude, request.Longitude);
                var distance = @case.Location.CalculateDistanceInMeters(deliveryLocation);

                if (distance > 500)
                {
                    await unitOfWork.RollbackTransactionAsync(ct);
                    return Result<bool>.Failure("يجب أن تكون في موقع الحالة لإتمام عملية التسليم.");
                }
            }

            // جلب حركات التعهد العيني المرتبطة بالعنصر والتي لم تُسلم بالكامل بعد
            var donations = await inKindDonationRepository.GetByCaseItemIdAsync(request.CaseItemId, ct);
            var donation = donations.FirstOrDefault(d => d.Status == DonationStatus.Pending || d.Status == DonationStatus.Verified);

            if (donation == null)
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                return Result<bool>.Failure("No active in-kind donation found for this item.");
            }

            // حفظ صورة إثبات التسليم ميدانياً
            var proofUrl = await fileService.SaveFileAsync(request.DeliveryPhoto.Stream, request.DeliveryPhoto.FileName, "deliveries", ct);

            // 🚀 التعديل 3: تسليم الكمية المحددة داخل الـ Domain Entity
            item.Fulfill(request.DeliveredQuantity);
            caseItemRepository.Update(item);

            // تحديث حركة التبرع (لو شغالين بنظام الكميات جوه الـ Donation برضه)
            donation.MarkAsDelivered(proofUrl); 
            inKindDonationRepository.Update(donation);

            // تحديث سجل الباحث الميداني والعدادات الخاصة به
            var researcher = await userRepository.GetByIdAsync(request.ConfirmedByUserId, ct);
            if (researcher != null)
            {
                researcher.IncrementVerifiedDeliveries();
                await userRepository.UpdateAsync(researcher);
            }

            // 🚀 التعديل 4: فحص إغلاق الحالة بناءً على تسليم كميات العناصر بالكامل
            var allItems = await caseItemRepository.GetAllAsync(i => i.CaseId == @case.Id, ct);
            bool allItemsDelivered = allItems.All(i => i.Status == CaseItemStatus.Delivered);

            if (allItemsDelivered)
            {
                @case.TransitionTo(CaseStatus.Closed, actorId: null);
                caseRepository.Update(@case);
                await caseUpdateService.NotifyCaseClosedAsync(@case.Id);
            }

            // 🚀 التعديل 5: مكافأة المتبرع بنقاط طردية محسوبة بناءً على الكمية التي وصلت فعلياً للمستفيد
            var donor = await userRepository.GetByIdAsync(donation.DonorId, ct);
            if (donor != null)
            {
                // كمثال: 10 نقاط عن كل وحدة تم تسليمها
                int pointsToAward = request.DeliveredQuantity * 10; 
                donor.AddPoints(pointsToAward);
                await userRepository.UpdateAsync(donor);

                var title = "Delivery Confirmed!";
                var message = $"شكراً لك! تم تسليم ({request.DeliveredQuantity}) من تبرعك لـ '{item.Name}' إلى مستحقيها بنجاح. لقد حصلت على {pointsToAward} نقطة أثر!";

                Guid donorId = donor.Id;
                var notification = new Notification(donorId, title, message);
                await notificationRepository.AddAsync(notification, ct);

                if (!string.IsNullOrEmpty(donor.DeviceToken))
                {
                    await fcmService.SendNotificationAsync(donor.DeviceToken, title, message);
                }
            }

            await unitOfWork.CommitTransactionAsync(ct);

            // تحديث الـ Dashboard الفورية للقرية عبر SignalR
            await caseUpdateService.NotifyVillageStatsUpdatedAsync();

            return Result<bool>.Success(true, "Delivery confirmed successfully.");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            return Result<bool>.Failure(ex.Message);
        }
    }
}