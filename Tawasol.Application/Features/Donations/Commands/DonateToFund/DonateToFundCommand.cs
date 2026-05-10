using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;
using Tawasol.Domain.Enums;

namespace Tawasol.Application.Features.Donations.Commands.DonateToFund;

public record DonateToFundCommand(
    Guid DonorId,
    decimal Amount,
    WalletCategory Category,
    FileModel ProofImage) : IRequest<Result<Guid>>;
