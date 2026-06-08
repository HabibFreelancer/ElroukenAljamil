namespace ElroukenAljamil.Application.DTOs;

public class CreateAnnonceDto
{
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
    public Dictionary<string, object>? ExtraData { get; set; }
}
