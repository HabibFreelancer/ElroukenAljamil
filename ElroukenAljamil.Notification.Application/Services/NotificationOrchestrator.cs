using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;

namespace ElroukenAljamil.Notification.Application.Services
{
    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IPreferenceRepository _preferenceRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly ITemplateRenderer _renderer;
        private readonly IEnumerable<INotificationSender> _senders;

        public NotificationOrchestrator(
            INotificationRepository notificationRepo,
            IPreferenceRepository preferenceRepo,
            ITemplateRepository templateRepo,
            ITemplateRenderer renderer,
            IEnumerable<INotificationSender> senders)
        {
            _notificationRepo = notificationRepo;
            _preferenceRepo = preferenceRepo;
            _templateRepo = templateRepo;
            _renderer = renderer;
            _senders = senders;
        }

        public async Task OrchestrateAsync(
            Guid recipientId,
            NotificationType type,
            string language,
            Dictionary<string, object> templateData,
            CancellationToken ct = default)
        {
            var channels = new[] { NotificationChannel.InApp, NotificationChannel.Email, NotificationChannel.Push, NotificationChannel.Sms };

            foreach (var channel in channels)
            {
                var pref = await _preferenceRepo.GetByUserAndTypeAsync(recipientId, type, ct);
                if (pref is not null && !pref.IsChannelEnabled(channel))
                    continue;

                var template = await _templateRepo.GetActiveAsync(type, channel, language, ct);
                if (template is null) continue;

                var title = await _renderer.RenderAsync(template.TitleTemplate, templateData, ct);
                var body = await _renderer.RenderAsync(template.BodyTemplate, templateData, ct);

                var notification = NotificationRecord.Create(recipientId, type, channel, title, body);
                await _notificationRepo.AddAsync(notification, ct);
            }
        }
    }
}
