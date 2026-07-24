using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class Feedback : AggregateRoot
{
    public new int Id { get; set; }
    public int? AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty; // tres_facile, facile, neutre, difficile, tres_difficile
    public string Category { get; set; } = string.Empty;

    public Annonce? Annonce { get; set; }
}
