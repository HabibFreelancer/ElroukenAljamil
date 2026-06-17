namespace ElroukenAljamil.Domain.Entities;

public class AnnonceFavorite
{
    public int Id { get; set; }
    public int AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
