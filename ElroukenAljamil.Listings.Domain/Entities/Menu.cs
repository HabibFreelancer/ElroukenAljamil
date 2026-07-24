using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class AnnonceMenu : AggregateRoot
{
    public new int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<AnnonceCategory> Categories { get; private set; } = new List<AnnonceCategory>();

    private AnnonceMenu() { }

    public static AnnonceMenu Create(string name, string slug, int displayOrder, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Le slug est obligatoire.", nameof(slug));

        return new AnnonceMenu
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            DisplayOrder = displayOrder,
            Icon = icon,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, int displayOrder, string? icon, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        DisplayOrder = displayOrder;
        Icon = icon;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
