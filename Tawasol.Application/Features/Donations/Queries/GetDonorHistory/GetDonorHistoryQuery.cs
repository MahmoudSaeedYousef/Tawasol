using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;

namespace Tawasol.Application.Features.Donations.Queries.GetDonorHistory;

public record GetDonorHistoryQuery(Guid DonorId) : IRequest<Result<IEnumerable<Transaction>>>;
