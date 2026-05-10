using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;

namespace Tawasol.Application.Features.Donations.Commands.ConfirmDelivery;

public record ConfirmDeliveryCommand(
    Guid DonationId,
    FileModel ProofOfDelivery) : IRequest<Result<bool>>;
