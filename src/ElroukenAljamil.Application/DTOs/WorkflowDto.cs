namespace ElroukenAljamil.Application.DTOs;

public class WorkflowDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkflowStepDto> Steps { get; set; } = new();
}

public class WorkflowStepDto
{
    public int Id { get; set; }
    public int StepOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string StepKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public List<StepFieldDto> Fields { get; set; } = new();
}

public class StepFieldDto
{
    public int Id { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public List<FieldOptionDto> Options { get; set; } = new();
    public string DefaultValue { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string HelperText { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public int? MaxLength { get; set; }
    public string VisibilityCondition { get; set; } = string.Empty;
}

public class FieldOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
