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
using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public class CancelPledgeInKindDonationCommandHandler(
    ICaseRepository caseRepository,
    IInKindDonationRepository inKindDonationRepository,
    IUnitOfWork unitOfWork,
    ICaseUpdateService caseUpdateService)
    : IRequestHandler<CancelPledgeInKindDonationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CancelPledgeInKindDonationCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null) return Result<Guid>.Failure("Case not found.");

        var item = @case.Items.FirstOrDefault(i => i.Id == request.CaseItemId);
        if (item == null) return Result<Guid>.Failure("Item not found in this case.");

        try
        {
            item.RevertPledge(request.Quantity);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
        var donations = await inKindDonationRepository.GetByCaseItemIdAsync(request.CaseItemId, ct);
        var donation = donations.FirstOrDefault(d => d.Status == DonationStatus.Pending || d.Status == DonationStatus.Verified);
        


        inKindDonationRepository.Update(donation, ct);
        // حفظ التغييرات في سياق واحد مجمع (AppDbContext الموحد)
        await unitOfWork.SaveChangesAsync(ct);

        // إرسال إشعار فوري وتحديث فوري للموبايل عبر SignalR (صامت وسلس)
        await caseUpdateService.NotifyNewPledgeAsync(request.CaseId, request.CaseItemId);

        return Result<Guid>.Success(donation.Id, "تم تسجيل التعهد بالكمية المطلوبة بنجاح.");
    }
}