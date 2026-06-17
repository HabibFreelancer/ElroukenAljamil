namespace ElroukenAljamil.Domain.Entities;

public class StepField
{
    public int Id { get; set; }
    public int StepId { get; set; }
    public string FieldKey { get; set; } = string.Empty; // "contract", "industry", "salary", "poste", etc.
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty; // "text", "number", "select", "textarea", "radio", "toggle"
    public string Placeholder { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty; // JSON array for select/radio options
    public string DefaultValue { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty; // e.g. "TND", "H/F"
    public string HelperText { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxLength { get; set; }
    public string ValidationRegex { get; set; } = string.Empty;
    public string VisibilityCondition { get; set; } = string.Empty; // JSON: {"field":"propertyType","values":["maison","autre"]}
    public WorkflowStep? Step { get; set; }
}
