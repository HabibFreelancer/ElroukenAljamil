using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class Annonce : AggregateRoot
{
    public new int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string AdType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool HidePhone { get; set; }
    public string ExtraData { get; set; } = "{}";
    public string Status { get; set; } = "published"; // draft, published, paused
    public int? CurrentStep { get; set; }
    public string UserId { get; set; } = string.Empty;

    public AnnonceCategory? Category { get; set; }
    public ICollection<AnnonceFavorite> Favorites { get; set; } = new List<AnnonceFavorite>();
    public ICollection<AnnonceView> Views { get; set; } = new List<AnnonceView>();
}
