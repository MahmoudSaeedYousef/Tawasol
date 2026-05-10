using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;

namespace Tawasol.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler(INotificationRepository notificationRepository)
    : IRequestHandler<GetMyNotificationsQuery, Result<IEnumerable<Notification>>>
{
    public async Task<Result<IEnumerable<Notification>>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var notifications = await notificationRepository.GetByUserIdAsync(request.UserId, ct);
        return Result<IEnumerable<Notification>>.Success(notifications);
    }
}
