using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class ListingAdType : AggregateRoot
{
    public new int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDefault { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ListingCategory? Category { get; private set; }

    private ListingAdType() { }

    public static ListingAdType Create(int categoryId, string label, string description,
        int displayOrder, bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Le libellé est obligatoire.", nameof(label));

        return new ListingAdType
        {
            CategoryId = categoryId,
            Label = label.Trim(),
            Description = description.Trim(),
            DisplayOrder = displayOrder,
            IsDefault = isDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int categoryId, string label, string description,
        int displayOrder, bool isDefault, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Le libellé est obligatoire.", nameof(label));
        CategoryId = categoryId;
        Label = label.Trim();
        Description = description.Trim();
        DisplayOrder = displayOrder;
        IsDefault = isDefault;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
