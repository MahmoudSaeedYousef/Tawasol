using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.DTOs.Admin;

namespace Tawasol.Application.Features.Admin.Queries.GetSystemFinanceSummary;

public record GetSystemFinanceSummaryQuery() : IRequest<Result<FinanceSummaryDto>>;
