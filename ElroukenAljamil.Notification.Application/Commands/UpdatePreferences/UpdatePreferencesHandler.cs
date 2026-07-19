using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.UpdatePreferences
{
    public class UpdatePreferencesHandler : IRequestHandler<UpdatePreferencesCommand, bool>
    {
        private readonly IPreferenceRepository _repository;

        public UpdatePreferencesHandler(IPreferenceRepository repository)
            => _repository = repository;

        public async Task<bool> Handle(UpdatePreferencesCommand request, CancellationToken ct)
        {
            var pref = await _repository.GetByUserAndTypeAsync(request.UserId, request.NotificationType, ct);

            if (pref is null)
            {
                pref = UserNotificationPreference.CreateDefault(request.UserId, request.NotificationType);
                pref.Update(request.EmailEnabled, request.SmsEnabled, request.PushEnabled, request.InAppEnabled);
                await _repository.AddAsync(pref, ct);
            }
            else
            {
                pref.Update(request.EmailEnabled, request.SmsEnabled, request.PushEnabled, request.InAppEnabled);
                await _repository.UpdateAsync(pref, ct);
            }

            return true;
        }
    }
}
