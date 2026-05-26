using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Donations.Commands.PledgeInKindDonation;

public record PledgeInKindDonationCommand(
    Guid DonorId,
    Guid CaseId,
    Guid CaseItemId,
    int Quantity, // 🚀 القطعة الناقصة: الكمية اللي المتبرع اختار يتعهد بيها
    ItemCondition Condition,
    string? EvidencePhotoUrl = null
) : IRequest<Result<Guid>>;
