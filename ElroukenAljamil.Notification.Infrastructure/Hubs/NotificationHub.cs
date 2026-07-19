using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Hubs
{
    /// <summary>
    /// Hub SignalR pour les notifications temps réel.
    /// Les clients se connectent via WebSocket et reçoivent les notifications instantanément.
    /// 
    /// Connexion côté client :
    ///   const connection = new signalR.HubConnectionBuilder()
    ///     .withUrl("/hubs/notifications", { accessTokenFactory: () => getJwtToken() })
    ///     .build();
    ///   connection.on("ReceiveNotification", (notification) => { ... });
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserConnectionTracker _connectionTracker;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(
            IUserConnectionTracker connectionTracker,
            ILogger<NotificationHub> logger)
        {
            _connectionTracker = connectionTracker;
            _logger = logger;
        }

        /// <summary>
        /// Appelé quand un client se connecte au Hub.
        /// Enregistre la connexion pour pouvoir cibler l'utilisateur ultérieurement.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                // Enregistrer cette connexion (un user peut avoir plusieurs devices)
                await _connectionTracker.AddConnectionAsync(userId, Context.ConnectionId);

                // Ajouter l'utilisateur à un groupe portant son ID
                // Cela permet d'envoyer à toutes ses connexions en une seule fois
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());

                _logger.LogInformation(
                    "Utilisateur {UserId} connecté (ConnectionId: {ConnId}).",
                    userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Appelé quand un client se déconnecte.
        /// Retire la connexion du tracker.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                await _connectionTracker.RemoveConnectionAsync(userId, Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());

                _logger.LogInformation(
                    "Utilisateur {UserId} déconnecté (ConnectionId: {ConnId}).",
                    userId, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Méthode appelable par le client pour marquer une notification comme lue.
        /// Évite un appel REST séparé pour cette action fréquente.
        /// </summary>
        public async Task MarkAsRead(Guid notificationId)
        {
            var userId = GetUserId();
            _logger.LogDebug(
                "Utilisateur {UserId} a lu la notification {NotifId}.",
                userId, notificationId);

            // L'événement sera traité côté serveur via le handler existant
            await Clients.Caller.SendAsync("NotificationMarkedRead", notificationId);
        }

        /// <summary>
        /// Méthode appelable par le client pour signaler sa présence (heartbeat).
        /// Permet de détecter les connexions zombies.
        /// </summary>
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        /// <summary>
        /// Extrait le UserId depuis le token JWT du contexte de connexion.
        /// </summary>
        private Guid GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var userId) ? userId : Guid.Empty;
        }
    }

}
