namespace ElroukenAljamil.Domain.Entities;

public class AnnonceView
{
    public int Id { get; set; }
    public int AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}
