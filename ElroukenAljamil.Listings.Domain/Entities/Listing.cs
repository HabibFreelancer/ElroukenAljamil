using System.Net;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Domain.Events;
using ElroukenAljamil.Listings.Domain.ValueObjects;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Exceptions;


namespace ElroukenAljamil.Listings.Domain.Entities
{
    /// <summary>
    /// Agrégat racine représentant une annonce sur la marketplace.
    /// Hérite de AggregateRoot<Guid> défini dans BuildingBlocks.Common.
    /// </summary>

    public class Listing : AggregateRoot
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public Category Category { get; private set; } = null!;
        public Location Location { get; private set; } = null!;
        public ListingStatus Status { get; private set; } = ListingStatus.Draft;
        public Guid SellerId { get; private set; }
        public string SellerName { get; private set; } = string.Empty;
        public List<string> ImageUrls { get; private set; } = new();
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public int ViewCount { get; private set; }

        private Listing() { } // EF Core

        /// <summary>
        /// Factory method pour créer une annonce. Encapsule les règles métier de création.
        /// </summary>
        public static Listing Create(
            string title,
            string description,
            Money price,
            Category category,
            Location location,
            Guid sellerId,
            string sellerName,
            List<string>? imageUrls = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Le titre est obligatoire.", nameof(title));
            if (title.Length > 200)
                throw new ArgumentException("Le titre ne peut pas dépasser 200 caractères.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La description est obligatoire.", nameof(description));
            if (sellerId == Guid.Empty)
                throw new ArgumentException("Le vendeur est obligatoire.", nameof(sellerId));

            var listing = new Listing
            {
                Id = Guid.NewGuid(),
                Title = title.Trim(),
                Description = description.Trim(),
                Price = price,
                Category = category,
                Location = location,
                SellerId = sellerId,
                SellerName = sellerName.Trim(),
                ImageUrls = imageUrls ?? new List<string>(),
                Status = ListingStatus.Active,
                PublishedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            listing.AddDomainEvent(new ListingCreatedDomainEvent(
                listing.Id, listing.Title, listing.SellerId));

            return listing;
        }

        /// <summary>
        /// Met à jour les informations de l'annonce.
        /// </summary>
        public void Update(
            string title,
            string description,
            Money price,
            Category category,
            Location location,
            List<string>? imageUrls = null)
        {
            if (Status == ListingStatus.Deactivated)
                throw new InvalidOperationException("Impossible de modifier une annonce désactivée.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Le titre est obligatoire.", nameof(title));
            if (title.Length > 200)
                throw new ArgumentException("Le titre ne peut pas dépasser 200 caractères.", nameof(title));

            Title = title.Trim();
            Description = description.Trim();
            Price = price;
            Category = category;
            Location = location;
            ImageUrls = imageUrls ?? ImageUrls;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ListingUpdatedDomainEvent(Id, Title, SellerId));
        }

        /// <summary>
        /// Désactive l'annonce (retrait de la marketplace).
        /// </summary>
        public void Deactivate()
        {
            if (Status == ListingStatus.Deactivated)
                throw new InvalidOperationException("L'annonce est déjà désactivée.");

            Status = ListingStatus.Deactivated;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ListingDeactivatedDomainEvent(Id, SellerId));
        }

        /// <summary>
        /// Marque l'annonce comme vendue.
        /// </summary>
        public void MarkAsSold()
        {
            if (Status != ListingStatus.Active)
                throw new InvalidOperationException("Seule une annonce active peut être marquée comme vendue.");

            Status = ListingStatus.Sold;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new ListingSoldDomainEvent(Id, SellerId));
        }

        /// <summary>
        /// Renouvelle l'annonce pour 30 jours supplémentaires.
        /// </summary>
        public void Renew()
        {
            if (Status != ListingStatus.Active && Status != ListingStatus.Expired)
                throw new InvalidOperationException("Seule une annonce active ou expirée peut être renouvelée.");

            Status = ListingStatus.Active;
            ExpiresAt = DateTime.UtcNow.AddDays(30);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Incrémente le compteur de vues.
        /// </summary>
        public void IncrementViewCount()
        {
            ViewCount++;
        }

        /// <summary>
        /// Vérifie si l'annonce est expirée.
        /// </summary>
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

        /// <summary>
        /// Marque comme expirée si la date d'expiration est dépassée.
        /// </summary>
        public void CheckExpiration()
        {
            if (IsExpired && Status == ListingStatus.Active)
            {
                Status = ListingStatus.Expired;
                UpdatedAt = DateTime.UtcNow;
                AddDomainEvent(new ListingExpiredDomainEvent(Id, SellerId));
            }
        }

    }
}
