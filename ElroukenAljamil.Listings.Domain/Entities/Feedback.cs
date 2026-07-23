namespace ElroukenAljamil.Listings.Domain.Entities;

public class Feedback
{
    public int Id { get; set; }
    public int? AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty; // tres_facile, facile, neutre, difficile, tres_difficile
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Annonce? Annonce { get; set; }
}
