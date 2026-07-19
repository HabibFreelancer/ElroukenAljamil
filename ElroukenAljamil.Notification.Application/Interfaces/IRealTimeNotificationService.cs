using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Application.Interfaces
{
    /// <summary>
    /// Service pour l'envoi de notifications en temps réel via WebSocket (SignalR).
    /// Pousse instantanément les notifications aux clients connectés.
    /// </summary>
    public interface IRealTimeNotificationService
    {
        /// <summary>
        /// Envoie une notification temps réel à un utilisateur connecté.
        /// </summary>
        /// <param name="userId">Destinataire</param>
        /// <param name="notification">Payload de la notification</param>
        /// <param name="ct">Token d'annulation</param>
        /// <returns>True si l'utilisateur était connecté et a reçu la notification</returns>
        Task<bool> SendToUserAsync(Guid userId, RealTimeNotificationPayload notification, CancellationToken ct = default);

        /// <summary>
        /// Envoie une notification à tous les utilisateurs connectés (broadcast admin).
        /// </summary>
        Task SendToAllAsync(RealTimeNotificationPayload notification, CancellationToken ct = default);

        /// <summary>
        /// Vérifie si un utilisateur est actuellement connecté.
        /// </summary>
        Task<bool> IsUserOnlineAsync(Guid userId, CancellationToken ct = default);
    }

    /// <summary>
    /// Payload envoyé au client via SignalR.
    /// </summary>
    public record RealTimeNotificationPayload
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public string? Metadata { get; init; }
        public DateTime CreatedAt { get; init; }
    }

}
