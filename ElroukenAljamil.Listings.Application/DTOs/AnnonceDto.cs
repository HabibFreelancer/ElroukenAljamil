namespace ElroukenAljamil.Listings.Application.DTOs;

public class AnnonceDto
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
    public string ExtraData { get; set; } = "{}";
    public string Status { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Menu { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Favorites { get; set; }
}

public class CreateAnnonceRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? AdType { get; set; }
    public string? Condition { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool HidePhone { get; set; }
    public Dictionary<string, object>? ExtraData { get; set; }
}

public class CreateDraftRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? AdType { get; set; }
    public string? Condition { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool HidePhone { get; set; }
    public int CurrentStep { get; set; }
    public Dictionary<string, object>? ExtraData { get; set; }
}

public class PriceEstimateRequest
{
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}

public class TrackViewRequest
{
    public string? UserId { get; set; }
}

public class PriceEstimateDto
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal AvgPrice { get; set; }
    public int Count { get; set; }
    public List<SimilarAdDto> SimilarAds { get; set; } = [];
}

public class SimilarAdDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Mileage { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

public class AnnonceDetailDto
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
    public string ExtraData { get; set; } = "{}";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Menu { get; set; } = string.Empty;
    public bool IsImmobilier { get; set; }
    public bool IsColocation { get; set; }
    public Dictionary<string, string>? ImmobilierDetails { get; set; }
    public int Views { get; set; }
    public int Favorites { get; set; }
}
