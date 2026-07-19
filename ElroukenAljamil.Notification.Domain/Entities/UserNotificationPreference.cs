using ElroukenAljamil.BuildingBlocks.Common;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Entities
{
    /// <summary>
    /// Entité représentant les préférences de notification d'un utilisateur pour un type donné.
    /// 
    /// Chaque utilisateur a une préférence par type de notification.
    /// Pour chaque type, il peut activer/désactiver indépendamment chaque canal.
    /// 
    /// Exemple pour l'utilisateur "Jean" :
    ///   NewMessage   → Email ✅ | SMS ❌ | Push ✅ | InApp ✅
    ///   ListingExpired → Email ✅ | SMS ✅ | Push ❌ | InApp ✅
    ///   Welcome      → Email ✅ | SMS ❌ | Push ❌ | InApp ✅
    /// 
    /// Si aucune préférence n'existe pour un type, les valeurs par défaut s'appliquent :
    ///   Email=true, SMS=false, Push=true, InApp=true
    /// 
    /// Contrainte d'unicité : un seul enregistrement par (UserId, NotificationType).
    /// </summary>
    public class UserNotificationPreference : BaseEntity
    {
        /// <summary>
        /// Identifiant de l'utilisateur propriétaire de cette préférence.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Type de notification concerné par cette ligne de préférence.
        /// Chaque type a sa propre configuration de canaux.
        /// </summary>
        public NotificationType NotificationType { get; private set; }

        /// <summary>
        /// Indique si l'envoi par email est activé pour ce type.
        /// Par défaut : true (les emails sont le canal principal).
        /// </summary>
        public bool EmailEnabled { get; private set; } = true;

        /// <summary>
        /// Indique si l'envoi par SMS est activé pour ce type.
        /// Par défaut : false (le SMS a un coût, l'utilisateur doit l'activer explicitement).
        /// </summary>
        public bool SmsEnabled { get; private set; } = false;

        /// <summary>
        /// Indique si les push notifications mobiles sont activées pour ce type.
        /// Par défaut : true (gratuit via Firebase).
        /// </summary>
        public bool PushEnabled { get; private set; } = true;

        /// <summary>
        /// Indique si la notification in-app est activée pour ce type.
        /// Par défaut : true (toujours activé, c'est le canal de base).
        /// </summary>
        public bool InAppEnabled { get; private set; } = true;

        private UserNotificationPreference() { } // Constructeur privé pour EF Core

        /// <summary>
        /// Crée une préférence avec les valeurs par défaut pour un type donné.
        /// Appelé lors du premier accès aux préférences d'un utilisateur
        /// ou lors de l'inscription (pré-population).
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="type">Type de notification</param>
        /// <returns>Nouvelle préférence avec les défauts</returns>
        public static UserNotificationPreference CreateDefault(Guid userId, NotificationType type)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("L'utilisateur est obligatoire.", nameof(userId));

            return new UserNotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                NotificationType = type,
                EmailEnabled = true,
                SmsEnabled = false,  // SMS désactivé par défaut (coût par envoi)
                PushEnabled = true,
                InAppEnabled = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Met à jour les canaux activés pour cette préférence.
        /// Appelé depuis le handler UpdatePreferencesCommand quand l'utilisateur
        /// modifie ses paramètres dans l'interface.
        /// </summary>
        /// <param name="email">Activer l'email</param>
        /// <param name="sms">Activer le SMS</param>
        /// <param name="push">Activer les push notifications</param>
        /// <param name="inApp">Activer les notifications in-app</param>
        public void Update(bool email, bool sms, bool push, bool inApp)
        {
            EmailEnabled = email;
            SmsEnabled = sms;
            PushEnabled = push;
            InAppEnabled = inApp;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Vérifie si un canal spécifique est activé dans cette préférence.
        /// Utilisé par le NotificationDispatcher pour déterminer sur quels canaux envoyer.
        /// </summary>
        /// <param name="channel">Canal à vérifier</param>
        /// <returns>True si le canal est activé pour ce type</returns>
        public bool IsChannelEnabled(NotificationChannel channel) => channel switch
        {
            NotificationChannel.Email => EmailEnabled,
            NotificationChannel.Sms => SmsEnabled,
            NotificationChannel.Push => PushEnabled,
            NotificationChannel.InApp => InAppEnabled,
            _ => false
        };
    }
}
