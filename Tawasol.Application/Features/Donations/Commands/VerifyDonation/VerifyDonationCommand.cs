using MediatR;
using Tawasol.Application.Common.Models;

namespace Tawasol.Application.Features.Donations.Commands.VerifyDonation;

public record VerifyDonationCommand(Guid TransactionId) : IRequest<Result<bool>>;
