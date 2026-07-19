using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Entities
{
    /// <summary>
    /// Entité représentant la configuration de digest email d'un utilisateur.
    /// Un digest regroupe les notifications non-lues en un seul email périodique,
    /// évitant de spammer l'utilisateur avec des emails individuels.
    /// </summary>
    public class DigestSchedule : BaseEntity
    {
        /// <summary>
        /// Utilisateur concerné par ce digest.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Fréquence du digest : quotidien ou hebdomadaire.
        /// </summary>
        public DigestFrequency Frequency { get; private set; } = DigestFrequency.Daily;

        /// <summary>
        /// Heure préférée d'envoi (0-23). Par défaut 8h du matin.
        /// </summary>
        public int PreferredHour { get; private set; } = 8;

        /// <summary>
        /// Jour préféré pour le digest hebdomadaire (0=Dimanche, 1=Lundi, etc.).
        /// Ignoré si la fréquence est quotidienne.
        /// </summary>
        public DayOfWeek PreferredDay { get; private set; } = DayOfWeek.Monday;

        /// <summary>
        /// Date/heure du dernier digest envoyé.
        /// Utilisé pour calculer les notifications à inclure dans le prochain digest.
        /// </summary>
        public DateTime? LastSentAt { get; private set; }

        /// <summary>
        /// Indique si le digest est actif.
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Fuseau horaire de l'utilisateur (pour envoyer à l'heure locale).
        /// </summary>
        public string TimeZone { get; private set; } = "Europe/Paris";

        private DigestSchedule() { } // EF Core

        /// <summary>
        /// Crée un nouveau digest schedule avec les paramètres par défaut.
        /// </summary>
        public static DigestSchedule Create(
            Guid userId,
            DigestFrequency frequency = DigestFrequency.Daily,
            int preferredHour = 8,
            DayOfWeek preferredDay = DayOfWeek.Monday,
            string timeZone = "Europe/Paris")
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("L'utilisateur est obligatoire.", nameof(userId));
            if (preferredHour < 0 || preferredHour > 23)
                throw new ArgumentException("L'heure doit être entre 0 et 23.", nameof(preferredHour));

            return new DigestSchedule
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Frequency = frequency,
                PreferredHour = preferredHour,
                PreferredDay = preferredDay,
                TimeZone = timeZone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Met à jour la configuration du digest.
        /// </summary>
        public void Update(DigestFrequency frequency, int preferredHour, DayOfWeek preferredDay, string timeZone)
        {
            Frequency = frequency;
            PreferredHour = preferredHour;
            PreferredDay = preferredDay;
            TimeZone = timeZone;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Enregistre qu'un digest a été envoyé.
        /// </summary>
        public void MarkAsSent()
        {
            LastSentAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Détermine si le digest doit être envoyé maintenant.
        /// </summary>
        public bool ShouldSendNow(DateTime utcNow)
        {
            if (!IsActive) return false;

            // Convertir l'heure UTC en heure locale de l'utilisateur
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);

                // Vérifier l'heure
                if (localNow.Hour != PreferredHour) return false;

                // Vérifier le jour pour les digests hebdomadaires
                if (Frequency == DigestFrequency.Weekly && localNow.DayOfWeek != PreferredDay)
                    return false;

                // Vérifier qu'on n'a pas déjà envoyé aujourd'hui
                if (LastSentAt.HasValue)
                {
                    var lastLocalSent = TimeZoneInfo.ConvertTimeFromUtc(LastSentAt.Value, tz);
                    if (lastLocalSent.Date == localNow.Date) return false;
                }

                return true;
            }
            catch
            {
                // Si le fuseau horaire est invalide, fallback sur UTC
                return utcNow.Hour == PreferredHour &&
                       (Frequency != DigestFrequency.Weekly || utcNow.DayOfWeek == PreferredDay);
            }
        }

        /// <summary>
        /// Active le digest.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Désactive le digest.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
