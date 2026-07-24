using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class WorkflowStep : BaseEntity
{
    public new int Id { get; private set; }
    public int WorkflowId { get; private set; }
    public int StepOrder { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Subtitle { get; private set; } = string.Empty;
    public string StepKey { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    public DepositWorkflow? Workflow { get; private set; }
    public ICollection<StepField> Fields { get; private set; } = new List<StepField>();

    private WorkflowStep() { }

    public static WorkflowStep Create(int workflowId, int stepOrder, string title,
        string subtitle, string stepKey, bool isRequired = true)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Le titre est obligatoire.", nameof(title));

        return new WorkflowStep
        {
            WorkflowId = workflowId,
            StepOrder = stepOrder,
            Title = title.Trim(),
            Subtitle = subtitle.Trim(),
            StepKey = stepKey.Trim(),
            IsRequired = isRequired,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(int stepOrder, string title, string subtitle, string stepKey, bool isRequired, bool isActive)
    {
        StepOrder = stepOrder;
        Title = title.Trim();
        Subtitle = subtitle.Trim();
        StepKey = stepKey.Trim();
        IsRequired = isRequired;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
