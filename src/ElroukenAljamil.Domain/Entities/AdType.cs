namespace ElroukenAljamil.Domain.Entities;

public class AdType
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public Category? Category { get; set; }
}
