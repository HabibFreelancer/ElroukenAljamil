using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Application.Services
{
    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IPreferenceRepository _preferenceRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly ITemplateRenderer _renderer;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly ILogger<NotificationOrchestrator> _logger;

        public NotificationOrchestrator(
            INotificationRepository notificationRepo,
            IPreferenceRepository preferenceRepo,
            ITemplateRepository templateRepo,
            ITemplateRenderer renderer,
            IEnumerable<INotificationSender> senders,
            ILogger<NotificationOrchestrator> logger)
        {
            _notificationRepo = notificationRepo;
            _preferenceRepo = preferenceRepo;
            _templateRepo = templateRepo;
            _renderer = renderer;
            _senders = senders;
            _logger = logger;
        }

        public async Task OrchestrateAsync(
            Guid recipientId,
            NotificationType type,
            string language,
            Dictionary<string, object> templateData,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Orchestration notification {Type} pour destinataire {RecipientId}", type, recipientId);

            var channels = new[] { NotificationChannel.InApp, NotificationChannel.Email, NotificationChannel.Push, NotificationChannel.Sms };

            foreach (var channel in channels)
            {
                var pref = await _preferenceRepo.GetByUserAndTypeAsync(recipientId, type, ct);
                if (pref is not null && !pref.IsChannelEnabled(channel))
                {
                    _logger.LogDebug("Canal {Channel} désactivé par préférence pour {RecipientId} / {Type}", channel, recipientId, type);
                    continue;
                }

                var template = await _templateRepo.GetActiveAsync(type, channel, language, ct);
                if (template is null)
                {
                    _logger.LogWarning("Aucun template actif trouvé pour {Type} / {Channel} / {Language}", type, channel, language);
                    continue;
                }

                var title = await _renderer.RenderAsync(template.TitleTemplate, templateData, ct);
                var body = await _renderer.RenderAsync(template.BodyTemplate, templateData, ct);

                var notification = NotificationRecord.Create(recipientId, type, channel, title, body);
                await _notificationRepo.AddAsync(notification, ct);
                _logger.LogDebug("Notification {Type} créée via canal {Channel} pour {RecipientId}", type, channel, recipientId);
            }

            _logger.LogInformation("Orchestration notification {Type} terminée pour {RecipientId}", type, recipientId);
        }
    }
}
