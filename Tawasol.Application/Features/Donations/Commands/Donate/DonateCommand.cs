using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;

namespace Tawasol.Application.Features.Donations.Commands.Donate;

public record DonateCommand(
    Guid DonorId,
    decimal Amount,
    Guid? CaseId,
    FileModel ProofImage) : IRequest<Result<Guid>>;
