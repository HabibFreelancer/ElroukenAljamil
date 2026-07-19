using ElroukenAljamil.Notification.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Queries.GetUserPreferences
{
    public record GetUserPreferencesQuery(Guid UserId) : IRequest<IReadOnlyList<NotificationPreferenceDto>>;
}
