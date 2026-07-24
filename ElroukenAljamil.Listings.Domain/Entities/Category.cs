using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class AnnonceCategory : AggregateRoot
{
    public new int Id { get; private set; }
    public int MenuId { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsLink { get; private set; } = true;
    public bool ShowInDeposit { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public AnnonceMenu Menu { get; private set; } = null!;
    public AnnonceCategory? ParentCategory { get; private set; }
    public ICollection<AnnonceCategory> SubCategories { get; private set; } = new List<AnnonceCategory>();

    private AnnonceCategory() { }

    public static AnnonceCategory Create(int menuId, string name, string slug,
        int displayOrder, int? parentCategoryId = null, bool showInDeposit = true, bool isLink = true)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Le slug est obligatoire.", nameof(slug));

        return new AnnonceCategory
        {
            MenuId = menuId,
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            DisplayOrder = displayOrder,
            ParentCategoryId = parentCategoryId,
            ShowInDeposit = showInDeposit,
            IsLink = isLink,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, int displayOrder,
        int? parentCategoryId, bool showInDeposit, bool isLink, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        DisplayOrder = displayOrder;
        ParentCategoryId = parentCategoryId;
        ShowInDeposit = showInDeposit;
        IsLink = isLink;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
