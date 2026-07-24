using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class AnnonceFavorite : BaseEntity
{
    public new int Id { get; set; }
    public int AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Annonce? Annonce { get; set; }
}
