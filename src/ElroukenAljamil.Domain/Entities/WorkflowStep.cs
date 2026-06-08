namespace ElroukenAljamil.Domain.Entities;

public class WorkflowStep
{
    public int Id { get; set; }
    public int WorkflowId { get; set; }
    public int StepOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string StepKey { get; set; } = string.Empty; // unique key: "title", "photos", "details", "salary", "description", "location", "contact"
    public bool IsRequired { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DepositWorkflow? Workflow { get; set; }
    public ICollection<StepField> Fields { get; set; } = new List<StepField>();
}
