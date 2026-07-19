using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Services
{
    /// <summary>
    /// Implémentation du service de notifications temps réel via SignalR.
    /// Pousse les notifications instantanément aux utilisateurs connectés.
    /// Si l'utilisateur n'est pas connecté, la notification est simplement
    /// stockée en BDD et visible au prochain chargement de l'interface.
    /// </summary>
    public class SignalRNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserConnectionTracker _connectionTracker;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext,
            IUserConnectionTracker connectionTracker,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _connectionTracker = connectionTracker;
            _logger = logger;
        }

        public async Task<bool> SendToUserAsync(
            Guid userId,
            RealTimeNotificationPayload notification,
            CancellationToken ct = default)
        {
            // Vérifier si l'utilisateur est en ligne
            var isOnline = await _connectionTracker.IsOnlineAsync(userId);

            if (!isOnline)
            {
                _logger.LogDebug("Utilisateur {UserId} hors ligne, notification stockée uniquement.", userId);
                return false;
            }

            // Envoyer à toutes les connexions de l'utilisateur via le groupe
            await _hubContext.Clients
                .Group(userId.ToString())
                .SendAsync("ReceiveNotification", notification, ct);

            _logger.LogInformation(
                "Notification temps réel envoyée à {UserId}: {Title}.",
                userId, notification.Title);

            return true;
        }

        public async Task SendToAllAsync(
            RealTimeNotificationPayload notification,
            CancellationToken ct = default)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification, ct);
            _logger.LogInformation("Notification broadcast envoyée à tous: {Title}.", notification.Title);
        }

        public async Task<bool> IsUserOnlineAsync(Guid userId, CancellationToken ct = default)
        {
            return await _connectionTracker.IsOnlineAsync(userId);
        }
    }
}
