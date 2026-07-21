using System.Text.Json;

namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record WorkflowDto
    {
        public int Id { get; init; }
        public int CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public int StepsCount { get; init; }
        public List<WorkflowStepDto> Steps { get; init; } = new();
    }

    public record WorkflowStepDto
    {
        public int Id { get; init; }
        public int StepOrder { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
        public string StepKey { get; init; } = string.Empty;
        public bool IsRequired { get; init; }
        public bool IsActive { get; init; }
        public int FieldsCount { get; init; }
        public List<StepFieldDto> Fields { get; init; } = new();
    }

    public record StepFieldDto
    {
        public int Id { get; init; }
        public string FieldKey { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public string FieldType { get; init; } = string.Empty;
        public string Placeholder { get; init; } = string.Empty;
        public List<FieldOptionDto> Options { get; init; } = new();
        public string DefaultValue { get; init; } = string.Empty;
        public string Suffix { get; init; } = string.Empty;
        public string HelperText { get; init; } = string.Empty;
        public bool IsRequired { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
        public int? MaxLength { get; init; }
        public string ValidationRegex { get; init; } = string.Empty;
        public string VisibilityCondition { get; init; } = string.Empty;
    }

    public record FieldOptionDto
    {
        public string Value { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
    }

    public record CreateWorkflowRequest
    {
        public int CategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
    }

    public record UpdateWorkflowRequest
    {
        public int CategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }

    public record CreateStepRequest
    {
        public int StepOrder { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
        public string StepKey { get; init; } = string.Empty;
        public bool IsRequired { get; init; } = true;
    }

    public record UpdateStepRequest
    {
        public int StepOrder { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
        public string StepKey { get; init; } = string.Empty;
        public bool IsRequired { get; init; }
        public bool IsActive { get; init; }
    }

    public record CreateFieldRequest
    {
        public string FieldKey { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public string FieldType { get; init; } = string.Empty;
        public string Placeholder { get; init; } = string.Empty;
        public string Options { get; init; } = string.Empty;
        public string DefaultValue { get; init; } = string.Empty;
        public string Suffix { get; init; } = string.Empty;
        public string HelperText { get; init; } = string.Empty;
        public bool IsRequired { get; init; }
        public int DisplayOrder { get; init; }
        public int? MaxLength { get; init; }
        public string ValidationRegex { get; init; } = string.Empty;
        public string VisibilityCondition { get; init; } = string.Empty;
    }

    public record UpdateFieldRequest : CreateFieldRequest
    {
        public bool IsActive { get; init; } = true;
    }
}
