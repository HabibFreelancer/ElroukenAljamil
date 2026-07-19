using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Services
{
    /// <summary>
    /// Service qui génère et envoie les digest emails.
    /// Un digest regroupe toutes les notifications non-lues d'un utilisateur
    /// en un seul email structuré, organisé par type de notification.
    /// 
    /// Flux :
    /// 1. Le DigestBackgroundWorker appelle ProcessPendingDigestsAsync() toutes les heures
    /// 2. On récupère les DigestSchedules actifs dont l'heure correspond
    /// 3. Pour chaque utilisateur, on collecte les notifications non-lues depuis le dernier digest
    /// 4. On génère un email HTML groupé par catégorie
    /// 5. On envoie via IEmailSender
    /// </summary>
    public class DigestService : IDigestService
    {
        private readonly IDigestScheduleRepository _scheduleRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRecipientResolver _recipientResolver;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly ILogger<DigestService> _logger;

        public DigestService(
            IDigestScheduleRepository scheduleRepository,
            INotificationRepository notificationRepository,
            IRecipientResolver recipientResolver,
            IEmailSender emailSender,
            ITemplateRenderer templateRenderer,
            ILogger<DigestService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _notificationRepository = notificationRepository;
            _recipientResolver = recipientResolver;
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
            _logger = logger;
        }

        public async Task ProcessPendingDigestsAsync(CancellationToken ct = default)
        {
            var utcNow = DateTime.UtcNow;
            var activeSchedules = await _scheduleRepository.GetActiveSchedulesAsync(ct);

            var dueSchedules = activeSchedules.Where(s => s.ShouldSendNow(utcNow)).ToList();

            if (!dueSchedules.Any())
            {
                _logger.LogDebug("Aucun digest à envoyer pour l'heure actuelle.");
                return;
            }

            _logger.LogInformation("{Count} digests à envoyer.", dueSchedules.Count);

            foreach (var schedule in dueSchedules)
            {
                try
                {
                    await SendDigestForUserAsync(schedule, ct);
                    schedule.MarkAsSent();
                    await _scheduleRepository.UpdateAsync(schedule, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Erreur lors de l'envoi du digest pour l'utilisateur {UserId}.",
                        schedule.UserId);
                }
            }
        }

        /// <summary>
        /// Génère et envoie le digest pour un utilisateur spécifique.
        /// </summary>
        private async Task SendDigestForUserAsync(
            Domain.Entities.DigestSchedule schedule,
            CancellationToken ct)
        {
            // 1. Résoudre les infos du destinataire
            var recipient = await _recipientResolver.ResolveAsync(schedule.UserId, ct);
            if (recipient is null)
            {
                _logger.LogWarning("Destinataire {UserId} introuvable pour le digest.", schedule.UserId);
                return;
            }

            // 2. Récupérer les notifications non-lues depuis le dernier digest
            var since = schedule.LastSentAt ?? DateTime.UtcNow.AddDays(-1);
            var unreadNotifications = await _notificationRepository.GetUnreadByRecipientAsync(schedule.UserId, ct);

            // Ne garder que celles créées après le dernier digest
            var newNotifications = unreadNotifications
                .Where(n => n.CreatedAt > since)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            if (!newNotifications.Any())
            {
                _logger.LogDebug("Aucune notification nouvelle pour le digest de {UserId}.", schedule.UserId);
                return;
            }

            // 3. Regrouper par type
            var grouped = newNotifications
                .GroupBy(n => n.Type)
                .OrderByDescending(g => g.Count())
                .ToList();

            // 4. Rendre le template du digest
            var templateData = new Dictionary<string, object>
            {
                ["user_name"] = recipient.FullName,
                ["total_count"] = newNotifications.Count,
                ["groups"] = grouped.Select(g => new
                {
                    type = GetFrenchTypeName(g.Key),
                    count = g.Count(),
                    items = g.Take(5).Select(n => new
                    {
                        title = n.Title,
                        body = n.Body.Length > 100 ? n.Body[..100] + "..." : n.Body,
                        date = n.CreatedAt.ToString("dd/MM à HH:mm")
                    }).ToList()
                }).ToList(),
                ["frequency"] = schedule.Frequency == DigestFrequency.Daily ? "quotidien" : "hebdomadaire",
                ["period_start"] = since.ToString("dd/MM/yyyy"),
                ["period_end"] = DateTime.UtcNow.ToString("dd/MM/yyyy")
            };

            var htmlBody = await _templateRenderer.RenderAsync(GetDigestTemplate(), templateData);

            // 5. Envoyer l'email
            var subject = $"📬 Votre résumé {(schedule.Frequency == DigestFrequency.Daily ? "du jour" : "de la semaine")} — {newNotifications.Count} notification(s)";

            await _emailSender.SendAsync(recipient.Email, recipient.FullName, subject, htmlBody, ct);

            _logger.LogInformation(
                "Digest envoyé à {Email} : {Count} notifications regroupées.",
                recipient.Email, newNotifications.Count);
        }

        /// <summary>
        /// Template HTML inline pour le digest (en production, stocker en BDD).
        /// </summary>
        private static string GetDigestTemplate()
        {
            return @"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
  <h1 style='color: #2563eb;'>📬 Bonjour {{ user_name }}</h1>
  <p>Voici votre résumé {{ frequency }} du <strong>{{ period_start }}</strong> au <strong>{{ period_end }}</strong>.</p>
  <p>Vous avez <strong>{{ total_count }}</strong> notification(s) non lue(s) :</p>
  
  {{ for group in groups }}
  <div style='margin: 20px 0; padding: 15px; background: #f8fafc; border-radius: 8px; border-left: 4px solid #2563eb;'>
    <h3 style='margin: 0 0 10px 0; color: #1e40af;'>{{ group.type }} ({{ group.count }})</h3>
    {{ for item in group.items }}
    <div style='padding: 8px 0; border-bottom: 1px solid #e2e8f0;'>
      <strong>{{ item.title }}</strong>
      <p style='margin: 4px 0; color: #64748b; font-size: 14px;'>{{ item.body }}</p>
      <small style='color: #94a3b8;'>{{ item.date }}</small>
    </div>
    {{ end }}
  </div>
  {{ end }}

  <div style='margin-top: 30px; padding: 15px; background: #eff6ff; border-radius: 8px; text-align: center;'>
    <a href='https://marketplace.com/notifications' style='display: inline-block; padding: 12px 24px; background: #2563eb; color: white; text-decoration: none; border-radius: 6px;'>
      Voir toutes mes notifications
    </a>
  </div>

  <p style='margin-top: 30px; font-size: 12px; color: #94a3b8; text-align: center;'>
    Pour modifier la fréquence de ce résumé, rendez-vous dans vos <a href='https://marketplace.com/settings/notifications'>paramètres de notification</a>.
  </p>
</body>
</html>";
        }

        /// <summary>
        /// Traduit les types de notification en français pour l'affichage.
        /// </summary>
        private static string GetFrenchTypeName(NotificationType type) => type switch
        {
            NotificationType.NewMessage => "💬 Nouveaux messages",
            NotificationType.ListingPublished => "📢 Annonces publiées",
            NotificationType.ListingExpiring => "⏰ Annonces bientôt expirées",
            NotificationType.ListingExpired => "❌ Annonces expirées",
            NotificationType.Welcome => "👋 Bienvenue",
            NotificationType.MediaFailed => "🖼️ Problèmes d'images",
            NotificationType.SearchAlert => "🔔 Alertes de recherche",
            _ => "📋 Autres notifications"
        };
    }

}
