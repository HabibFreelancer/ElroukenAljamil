using ElroukenAljamil.BuildingBlocks.Common;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Entities
{
    /// <summary>
    /// Entité représentant un template de notification stocké en base de données.
    /// 
    /// Les templates utilisent la syntaxe Scriban (similaire à Liquid/Mustache) pour le rendu dynamique.
    /// Scriban est un moteur de template rapide et sécurisé (pas d'exécution de code arbitraire).
    /// 
    /// Syntaxe Scriban :
    ///   Variables : {{ variable_name }}
    ///   Conditions : {{ if condition }} ... {{ end }}
    ///   Boucles : {{ for item in collection }} ... {{ end }}
    ///   Filtres : {{ variable | string.upcase }}
    /// 
    /// Exemple de TitleTemplate :
    ///   "Nouveau message de {{ sender_name }}"
    /// 
    /// Exemple de BodyTemplate (email HTML) :
    ///   "<h1>Bonjour {{ user_name }}</h1>
    ///    <p>{{ sender_name }} vous a envoyé un message à propos de « {{ listing_title }} ».</p>
    ///    <p>Aperçu : {{ message_preview }}</p>"
    /// 
    /// Organisation :
    ///   - Un template par combinaison (Type + Channel + Language)
    ///   - Le template Email est en HTML, le SMS en texte brut, le Push est court
    ///   - Contrainte d'unicité : un seul template actif par (Type, Channel, Language)
    /// </summary>
    public class NotificationTemplate : BaseEntity
    {
        /// <summary>
        /// Type de notification pour lequel ce template est utilisé.
        /// Permet de charger le bon template selon l'événement métier.
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Canal ciblé par ce template.
        /// Un même type a des templates différents selon le canal :
        ///   - Email → HTML riche avec mise en page
        ///   - SMS → Texte court (160 caractères max pour 1 segment)
        ///   - Push → Titre + corps courts
        ///   - InApp → Texte moyen
        /// </summary>
        public NotificationChannel Channel { get; private set; }

        /// <summary>
        /// Template du titre/objet en syntaxe Scriban.
        /// Pour les emails : c'est l'objet (subject line).
        /// Pour le push : c'est le titre de la notification.
        /// Pour InApp : c'est le titre affiché dans la liste.
        /// </summary>
        public string TitleTemplate { get; private set; } = string.Empty;

        /// <summary>
        /// Template du corps en syntaxe Scriban.
        /// Pour les emails : HTML complet avec mise en page.
        /// Pour le SMS : texte brut (court).
        /// Pour le push : corps de la notification (1-2 lignes).
        /// Pour InApp : texte moyen affiché au clic.
        /// </summary>
        public string BodyTemplate { get; private set; } = string.Empty;

        /// <summary>
        /// Langue du template (code ISO 639-1).
        /// Permet le support multilingue futur : un template par langue.
        /// Par défaut "fr" pour le français.
        /// </summary>
        public string Language { get; private set; } = "fr";

        /// <summary>
        /// Indique si ce template est actif et doit être utilisé pour les envois.
        /// Permet de désactiver un template sans le supprimer (historique).
        /// Contrainte : un seul template actif par (Type, Channel, Language).
        /// </summary>
        public bool IsActive { get; private set; } = true;

        private NotificationTemplate() { } // Constructeur privé pour EF Core

        /// <summary>
        /// Crée un nouveau template de notification.
        /// </summary>
        /// <param name="type">Type de notification ciblé</param>
        /// <param name="channel">Canal ciblé</param>
        /// <param name="titleTemplate">Template Scriban pour le titre</param>
        /// <param name="bodyTemplate">Template Scriban pour le corps</param>
        /// <param name="language">Code langue (défaut: "fr")</param>
        /// <returns>Nouveau template actif</returns>
        public static NotificationTemplate Create(
            NotificationType type,
            NotificationChannel channel,
            string titleTemplate,
            string bodyTemplate,
            string language = "fr")
        {
            if (string.IsNullOrWhiteSpace(titleTemplate))
                throw new ArgumentException("Le template de titre est obligatoire.", nameof(titleTemplate));
            if (string.IsNullOrWhiteSpace(bodyTemplate))
                throw new ArgumentException("Le template de corps est obligatoire.", nameof(bodyTemplate));

            return new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Type = type,
                Channel = channel,
                TitleTemplate = titleTemplate,
                BodyTemplate = bodyTemplate,
                Language = language,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Met à jour le contenu du template.
        /// Appelé par un admin pour modifier le texte sans recréer l'enregistrement.
        /// </summary>
        /// <param name="titleTemplate">Nouveau template de titre</param>
        /// <param name="bodyTemplate">Nouveau template de corps</param>
        public void UpdateContent(string titleTemplate, string bodyTemplate)
        {
            if (string.IsNullOrWhiteSpace(titleTemplate))
                throw new ArgumentException("Le template de titre est obligatoire.", nameof(titleTemplate));
            if (string.IsNullOrWhiteSpace(bodyTemplate))
                throw new ArgumentException("Le template de corps est obligatoire.", nameof(bodyTemplate));

            TitleTemplate = titleTemplate;
            BodyTemplate = bodyTemplate;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Désactive le template. Il ne sera plus utilisé pour les envois.
        /// Utile pour faire une rotation de templates sans perdre l'historique.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Réactive un template précédemment désactivé.
        /// Attention : vérifier qu'il n'y a pas déjà un template actif pour le même (Type, Channel, Language).
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
