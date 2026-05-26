using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public record CancelPledgeInKindDonationCommand(
    Guid CaseId,
    Guid CaseItemId,
    int Quantity
) : IRequest<Result<Guid>>;