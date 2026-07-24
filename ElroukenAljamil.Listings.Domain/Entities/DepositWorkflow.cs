using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class DepositWorkflow : AggregateRoot
{
    public new int Id { get; private set; }
    public int CategoryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public AnnonceCategory? Category { get; private set; }
    public ICollection<WorkflowStep> Steps { get; private set; } = new List<WorkflowStep>();

    private DepositWorkflow() { }

    public static DepositWorkflow Create(int categoryId, string name, string description, bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));

        return new DepositWorkflow
        {
            CategoryId = categoryId,
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int categoryId, string name, string description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Le nom est obligatoire.", nameof(name));
        CategoryId = categoryId;
        Name = name.Trim();
        Description = description.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
