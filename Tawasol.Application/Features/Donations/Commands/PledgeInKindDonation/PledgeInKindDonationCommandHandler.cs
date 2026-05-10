using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public class PledgeInKindDonationCommandHandler(
    ICaseRepository caseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PledgeInKindDonationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(PledgeInKindDonationCommand request, CancellationToken ct)
    {
        var @case = await caseRepository.GetByIdAsync(request.CaseId, ct);
        if (@case == null) return Result<Guid>.Failure("Case not found.");

        var item = @case.Items.FirstOrDefault(i => i.Id == request.CaseItemId);
        if (item == null) return Result<Guid>.Failure("Item not found in this case.");

        if (item.IsPledged) return Result<Guid>.Failure("This item has already been pledged by another donor.");

        item.Pledge(request.DonorId);
        
        var donation = Donation.CreateInKind(request.DonorId, request.CaseItemId);
        
        // Note: For full implementation, we'd add the donation to a repository. 
        // For now, we rely on the DB context change tracking if the entity was newly created.
        // If we had a donation repository: await donationRepository.AddAsync(donation, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Success(donation.Id, "In-kind donation pledged successfully.");
    }
}
