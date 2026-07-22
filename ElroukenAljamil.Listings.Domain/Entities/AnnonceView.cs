namespace ElroukenAljamil.Listings.Domain.Entities;

public class AnnonceView
{
    public int Id { get; set; }
    public int AnnonceId { get; set; }
    public string UserId { get; set; } = "anonymous";
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    public Annonce? Annonce { get; set; }
}
