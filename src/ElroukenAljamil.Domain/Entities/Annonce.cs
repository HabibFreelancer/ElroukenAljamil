namespace ElroukenAljamil.Domain.Entities;

public class Annonce
{
    public int Id { get; set; }
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
    public string ExtraData { get; set; } = string.Empty; // JSON for workflow-specific fields
    public string Status { get; set; } = "published"; // draft, published
    public int? CurrentStep { get; set; } // For resuming draft deposits
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Category? Category { get; set; }
}
