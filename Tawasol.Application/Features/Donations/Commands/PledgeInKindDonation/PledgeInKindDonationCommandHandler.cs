using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Interfaces;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public class PledgeInKindDonationCommandHandler(
    ICaseRepository caseRepository,
    IInKindDonationRepository inKindDonationRepository,
    IUnitOfWork unitOfWork,
    ICaseUpdateService caseUpdateService)
    : IRequestHandler<PledgeInKindDonationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(PledgeInKindDonationCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null) return Result<Guid>.Failure("Case not found.");

        var item = @case.Items.FirstOrDefault(i => i.Id == request.CaseItemId);
        if (item == null) return Result<Guid>.Failure("Item not found in this case.");

        try
        {
            // 🚀 التعديل 1: التعهد بالكمية المطلوبة داخل الـ Domain ( DDD Encapsulation)
            // الميثود دي جواها هتحسب الـ RemainingAmount وتزود الـ PledgedAmount أوتوماتيكياً
            item.Pledge(request.Quantity);
        }
        catch (DomainException ex)
        {
            // لو الكمية المطلوبة أكبر من المتبقي، الـ Domain هيطرد الحركة ونرد بـ Failure صريح للموبايل
            return Result<Guid>.Failure(ex.Message);
        }

        // 🚀 التعديل 2: تمرير الـ Quantity لحركة التبرع العيني عشان تتسيف في الـ DB 
        // (تأكد من إضافة حقل Quantity في الـ Constructor بتاع الـ InKindDonation entity)
        var donation = new InKindDonation(
            request.DonorId, 
            request.CaseItemId, 
            request.Condition, 
            request.Quantity, 
            request.EvidencePhotoUrl
        );
        
        await inKindDonationRepository.AddAsync(donation, ct);

        // حفظ التغييرات في سياق واحد مجمع (AppDbContext الموحد)
        await unitOfWork.SaveChangesAsync(ct);

        // إرسال إشعار فوري وتحديث فوري للموبايل عبر SignalR (صامت وسلس)
        await caseUpdateService.NotifyNewPledgeAsync(request.CaseId, request.CaseItemId);

        return Result<Guid>.Success(donation.Id, "تم تسجيل التعهد بالكمية المطلوبة بنجاح.");
    }
}