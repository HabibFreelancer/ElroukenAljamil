using ElroukenAljamil.Notification.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Queries.GetUserNotifications
{
    public record GetUserNotificationsQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<NotificationDto>>;
}
