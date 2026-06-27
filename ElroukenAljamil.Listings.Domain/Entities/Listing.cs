using System.Net;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Domain.Events;
using ElroukenAljamil.Listings.Domain.Exceptions;
using ElroukenAljamil.Listings.Domain.ValueObjects;


namespace ElroukenAljamil.Listings.Domain.Entities
{
    /// <summary>
    /// Aggregate Root représentant une annonce sur la marketplace.
    /// Encapsule toute la logique métier liée à une annonce.
    /// </summary>
    public class Listing : BaseEntity
    {
        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public Money Price { get; private set; } = default!;
        public ListingStatus Status { get; private set; }
        public Guid SellerId { get; private set; }
        public Guid CategoryId { get; private set; }
        public Address Location { get; private set; } = default!;


        private readonly List<ListingImage> _images = new();
        public IReadOnlyCollection<ListingImage> Images => _images.AsReadOnly();


        // Constructeur privé pour EF Core
        private Listing() { }


        /// <summary>
        /// Factory method pour créer une annonce avec validation métier.
        /// </summary>
        public static Listing Create(
            string title,
            string description,
            Money price,
            Guid sellerId,
            Guid categoryId,
            Address location)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ListingDomainException("Le titre est obligatoire.");


            if (title.Length > 150)
                throw new ListingDomainException("Le titre ne peut pas dépasser 150 caractères.");


            if (string.IsNullOrWhiteSpace(description))
                throw new ListingDomainException("La description est obligatoire.");


            if (price.Amount <= 0)
                throw new ListingDomainException("Le prix doit être positif.");


            var listing = new Listing
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Price = price,
                SellerId = sellerId,
                CategoryId = categoryId,
                Location = location,
                Status = ListingStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };


            listing.AddDomainEvent(new ListingCreatedEvent(listing.Id, sellerId));


            return listing;
        }


        /// <summary>
        /// Publie l'annonce pour la rendre visible aux acheteurs.
        /// </summary>
        public void Publish()
        {
            if (Status != ListingStatus.Draft)
                throw new ListingDomainException("Seule une annonce en brouillon peut être publiée.");


            if (!_images.Any())
                throw new ListingDomainException("Au moins une image est requise pour publier.");


            Status = ListingStatus.Active;
            UpdatedAt = DateTime.UtcNow;


            AddDomainEvent(new ListingPublishedEvent(Id));
        }


        /// <summary>
        /// Désactive l'annonce (vendu ou retiré).
        /// </summary>
        public void Deactivate()
        {
            if (Status != ListingStatus.Active)
                throw new ListingDomainException("Seule une annonce active peut être désactivée.");


            Status = ListingStatus.Sold;
            UpdatedAt = DateTime.UtcNow;
        }


        /// <summary>
        /// Met à jour les informations de l'annonce.
        /// </summary>
        public void Update(string title, string description, Money price)
        {
            if (Status == ListingStatus.Sold)
                throw new ListingDomainException("Impossible de modifier une annonce vendue.");


            Title = !string.IsNullOrWhiteSpace(title) ? title : Title;
            Description = !string.IsNullOrWhiteSpace(description) ? description : Description;
            Price = price.Amount > 0 ? price : Price;
            UpdatedAt = DateTime.UtcNow;
        }


        /// <summary>
        /// Ajoute une image à l'annonce (max 10 images).
        /// </summary>
        public void AddImage(string url, int order)
        {
            if (_images.Count >= 10)
                throw new ListingDomainException("Maximum 10 images par annonce.");


            _images.Add(new ListingImage(Guid.NewGuid(), Id, url, order));
        }


        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image is null)
                throw new ListingDomainException("Image introuvable.");


            _images.Remove(image);
        }
    }

}
