using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public record PledgeInKindDonationCommand(
    Guid DonorId,
    Guid CaseId,
    Guid CaseItemId) : IRequest<Result<Guid>>;
