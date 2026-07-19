using ElroukenAljamil.BuildingBlocks.Common;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Events;

namespace ElroukenAljamil.Notification.Domain.Entities
{
    /// <summary>
    /// Agrégat racine représentant une notification envoyée à un utilisateur.
    /// Chaque notification correspond à un envoi sur un canal spécifique (email, SMS, push, in-app).
    /// Une même action métier peut générer plusieurs NotificationRecord (une par canal activé).
    /// 
    /// Exemple : un nouveau message reçu peut créer :
    ///   - 1 NotificationRecord (channel=InApp) → stocké en BDD, affiché dans l'interface
    ///   - 1 NotificationRecord (channel=Email) → envoyé via Brevo SMTP
    ///   - 1 NotificationRecord (channel=Push)  → envoyé via Firebase FCM
    /// 
    /// Cycle de vie :
    ///   Pending → Sent (succès) ou Pending → Failed (après 3 tentatives)
    ///   Sent → Read (uniquement pour InApp, quand l'utilisateur clique)
    /// </summary>
    public class NotificationRecord : AggregateRoot
    {
        /// <summary>
        /// Identifiant de l'utilisateur destinataire.
        /// </summary>
        public Guid RecipientId { get; private set; }

        /// <summary>
        /// Type de notification (NewMessage, ListingPublished, etc.).
        /// Permet de catégoriser et filtrer les notifications.
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Canal de diffusion utilisé pour cet envoi spécifique.
        /// </summary>
        public NotificationChannel Channel { get; private set; }

        /// <summary>
        /// Titre court affiché dans la liste in-app ou en objet d'email.
        /// Exemple : "Nouveau message de Jean"
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// Corps complet de la notification (HTML pour email, texte pour SMS/push/in-app).
        /// </summary>
        public string Body { get; private set; } = string.Empty;

        /// <summary>
        /// Statut actuel dans le cycle de vie.
        /// </summary>
        public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;

        /// <summary>
        /// Message d'erreur si l'envoi a échoué.
        /// Stocke la dernière erreur rencontrée (utile pour le debug admin).
        /// </summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// Nombre de tentatives d'envoi effectuées.
        /// Incrémenté à chaque échec. Après MaxRetries, passe en Failed définitif.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Date d'envoi effectif au provider (null si pas encore envoyé).
        /// </summary>
        public DateTime? SentAt { get; private set; }

        /// <summary>
        /// Date de lecture par l'utilisateur (uniquement pour les notifications InApp).
        /// </summary>
        public DateTime? ReadAt { get; private set; }

        /// <summary>
        /// Données additionnelles sérialisées en JSON.
        /// Contient les informations de contexte pour construire des liens côté frontend.
        /// Exemple : {"conversationId": "...", "listingId": "...", "action": "open_conversation"}
        /// </summary>
        public string? Metadata { get; private set; }

        /// <summary>
        /// Nombre maximum de tentatives avant de passer en échec définitif.
        /// </summary>
        private const int MaxRetries = 3;

        private NotificationRecord() { } // Constructeur privé pour EF Core

        /// <summary>
        /// Factory method pour créer une nouvelle notification prête à être envoyée.
        /// La notification est créée en statut Pending et sera traitée par le dispatcher.
        /// </summary>
        /// <param name="recipientId">Identifiant du destinataire</param>
        /// <param name="type">Type métier de la notification</param>
        /// <param name="channel">Canal d'envoi cible</param>
        /// <param name="title">Titre/Objet de la notification</param>
        /// <param name="body">Corps complet du message</param>
        /// <param name="metadata">Données JSON de contexte (optionnel)</param>
        /// <returns>Nouvelle instance de NotificationRecord</returns>
        public static NotificationRecord Create(
            Guid recipientId,
            NotificationType type,
            NotificationChannel channel,
            string title,
            string body,
            string? metadata = null)
        {
            // Validations des invariants métier
            if (recipientId == Guid.Empty)
                throw new ArgumentException("Le destinataire est obligatoire.", nameof(recipientId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Le titre est obligatoire.", nameof(title));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Le corps est obligatoire.", nameof(body));

            var notification = new NotificationRecord
            {
                Id = Guid.NewGuid(),
                RecipientId = recipientId,
                Type = type,
                Channel = channel,
                Title = title.Trim(),
                Body = body.Trim(),
                Status = NotificationStatus.Pending,
                RetryCount = 0,
                Metadata = metadata,
                CreatedAt = DateTime.UtcNow
            };

            // Émettre un domain event pour traçabilité interne
            notification.AddDomainEvent(new NotificationCreatedDomainEvent(
                notification.Id, recipientId, type, channel));

            return notification;
        }

        /// <summary>
        /// Marque la notification comme envoyée avec succès au provider.
        /// Idempotent : si déjà Sent, ne fait rien.
        /// </summary>
        public void MarkAsSent()
        {
            if (Status == NotificationStatus.Sent)
                return; // Idempotent — évite les doubles marquages

            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new NotificationSentDomainEvent(Id, RecipientId, Channel));
        }

        /// <summary>
        /// Marque la notification comme échouée.
        /// Incrémente le compteur de retry.
        /// Si le nombre max de tentatives est atteint, passe en échec définitif.
        /// Sinon, reste en Pending pour être retentée par le NotificationRetryWorker.
        /// </summary>
        /// <param name="errorMessage">Description technique de l'erreur (pour le debug)</param>
        public void MarkAsFailed(string errorMessage)
        {
            RetryCount++;
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;

            if (RetryCount >= MaxRetries)
            {
                // Échec définitif après 3 tentatives — on ne retente plus
                Status = NotificationStatus.Failed;
                AddDomainEvent(new NotificationFailedDomainEvent(Id, RecipientId, Channel, errorMessage));
            }
            else
            {
                // Reste en Pending → sera retentée par le background worker
                Status = NotificationStatus.Pending;
            }
        }

        /// <summary>
        /// Marque la notification in-app comme lue par l'utilisateur.
        /// Ne s'applique qu'au canal InApp (les emails/SMS n'ont pas de concept de "lu" côté serveur).
        /// Idempotent : si déjà lue, ne fait rien.
        /// </summary>
        public void MarkAsRead()
        {
            if (Channel != NotificationChannel.InApp)
                throw new InvalidOperationException(
                    "Seules les notifications in-app peuvent être marquées comme lues.");

            if (ReadAt.HasValue)
                return; // Déjà lue — idempotent

            ReadAt = DateTime.UtcNow;
            Status = NotificationStatus.Read;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Vérifie si la notification peut être retentée par le worker.
        /// True si elle est en Pending ET n'a pas épuisé ses tentatives.
        /// </summary>
        public bool CanRetry => Status == NotificationStatus.Pending && RetryCount < MaxRetries;

        /// <summary>
        /// Vérifie si la notification est dans un état terminal (aucune action possible).
        /// True si Sent, Failed ou Read.
        /// </summary>
        public bool IsTerminal => Status == NotificationStatus.Sent ||
                                  Status == NotificationStatus.Failed ||
                                  Status == NotificationStatus.Read;
    }
}
