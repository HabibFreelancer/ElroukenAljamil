using ElroukenAljamil.Notification.Domain.Enums;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.UpdatePreferences
{
    public record UpdatePreferencesCommand(
        Guid UserId,
        NotificationType NotificationType,
        bool EmailEnabled,
        bool SmsEnabled,
        bool PushEnabled,
        bool InAppEnabled
    ) : IRequest<bool>;
}
