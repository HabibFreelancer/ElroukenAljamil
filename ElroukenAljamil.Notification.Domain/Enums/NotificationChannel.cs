using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Domain.Enums
{
    /// <summary>
    /// Canaux de diffusion des notifications.
    /// Un utilisateur peut activer/désactiver chaque canal indépendamment
    /// via ses préférences (UserNotificationPreference).
    /// 
    /// Chaque canal a son propre provider d'envoi :
    /// - Email → BrevoEmailSender (SMTP via MailKit)
    /// - Sms → BrevoSmsSender (API REST Brevo)
    /// - Push → FirebasePushSender (Firebase Cloud Messaging)
    /// - InApp → Stocké en PostgreSQL, affiché dans l'interface + poussé via SignalR
    /// </summary>
    public enum NotificationChannel
    {
        Email = 0,    // Email via Brevo SMTP (300/jour gratuit)
        Sms = 1,     // SMS via Brevo API (crédits gratuits à l'inscription)
        Push = 2,    // Push notification mobile via Firebase FCM (gratuit illimité)
        InApp = 3    // Notification in-app (stockée en BDD, temps réel via SignalR)
    }
}
