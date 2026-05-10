using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;

namespace Tawasol.Application.Features.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(Guid UserId) : IRequest<Result<IEnumerable<Notification>>>;
