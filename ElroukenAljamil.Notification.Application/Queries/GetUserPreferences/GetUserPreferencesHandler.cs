using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Queries.GetUserPreferences
{
    public class GetUserPreferencesHandler : IRequestHandler<GetUserPreferencesQuery, IReadOnlyList<NotificationPreferenceDto>>
    {
        private readonly IPreferenceRepository _repository;

        public GetUserPreferencesHandler(IPreferenceRepository repository)
            => _repository = repository;

        public async Task<IReadOnlyList<NotificationPreferenceDto>> Handle(GetUserPreferencesQuery request, CancellationToken ct)
        {
            var prefs = await _repository.GetByUserAsync(request.UserId, ct);
            return prefs.Select(p => new NotificationPreferenceDto
            {
                NotificationType = p.NotificationType.ToString(),
                EmailEnabled = p.EmailEnabled,
                SmsEnabled = p.SmsEnabled,
                PushEnabled = p.PushEnabled,
                InAppEnabled = p.InAppEnabled
            }).ToList();
        }
    }
}
