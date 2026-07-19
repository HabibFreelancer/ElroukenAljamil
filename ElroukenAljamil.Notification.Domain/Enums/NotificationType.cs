using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Domain.Enums
{
    /// <summary>
    /// Types de notification métier.
    /// Chaque type correspond à un événement métier qui déclenche une notification.
    /// Utilisé pour :
    /// - Catégoriser les notifications dans l'interface utilisateur
    /// - Charger le bon template (un template par type + canal)
    /// - Permettre à l'utilisateur de désactiver certains types via ses préférences
    /// </summary>
    public enum NotificationType
    {
        // --- Compte utilisateur ---
        Welcome = 0,              // Inscription réussie → email de bienvenue
        EmailVerification = 1,    // Demande de vérification de l'adresse email
        PasswordReset = 2,        // Lien de réinitialisation du mot de passe
        AccountLocked = 3,        // Compte verrouillé après trop de tentatives échouées

        // --- Annonces ---
        ListingPublished = 10,    // Confirmation que l'annonce est en ligne
        ListingExpiring = 11,     // Rappel : l'annonce expire dans 3 jours
        ListingExpired = 12,      // L'annonce a expiré
        ListingSold = 13,         // L'annonce a été marquée comme vendue

        // --- Messagerie ---
        NewMessage = 20,          // Un nouveau message a été reçu dans une conversation
        MessageRead = 21,         // Le destinataire a lu le message (accusé de lecture)

        // --- Médias / Images ---
        MediaProcessed = 30,      // Image traitée avec succès (variantes générées)
        MediaFailed = 31,         // Échec du traitement d'une image

        // --- Alertes de recherche ---
        SearchAlert = 40,         // Nouvelle annonce correspondant à une alerte sauvegardée
        PriceDropAlert = 41       // Baisse de prix sur une annonce suivie
    }

}
