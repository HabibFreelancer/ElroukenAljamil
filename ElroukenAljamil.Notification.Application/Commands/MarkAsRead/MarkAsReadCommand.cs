using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<bool>;
}
