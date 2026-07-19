using ElroukenAljamil.Notification.Domain.Enums;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.SendNotification
{
    public record SendNotificationCommand(
        Guid RecipientId,
        NotificationType Type,
        string Language,
        Dictionary<string, object> TemplateData
    ) : IRequest<bool>;
}
