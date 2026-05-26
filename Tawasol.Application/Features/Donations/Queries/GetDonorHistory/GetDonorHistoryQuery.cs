using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Donations;

namespace Tawasol.Application.Features.Donations.Queries.GetDonorHistory;

public record GetDonorHistoryQuery(Guid DonorId) : IRequest<Result<IEnumerable<DonationHistoryDto>>>;
