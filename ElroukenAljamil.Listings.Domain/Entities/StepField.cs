using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class StepField : BaseEntity
{
    public new int Id { get; private set; }
    public int StepId { get; private set; }
    public string FieldKey { get; private set; } = string.Empty;
    public string Label { get; private set; } = string.Empty;
    public string FieldType { get; private set; } = string.Empty;
    public string Placeholder { get; private set; } = string.Empty;
    public string Options { get; private set; } = string.Empty; // JSON
    public string DefaultValue { get; private set; } = string.Empty;
    public string Suffix { get; private set; } = string.Empty;
    public string HelperText { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int? MaxLength { get; private set; }
    public string ValidationRegex { get; private set; } = string.Empty;
    public string VisibilityCondition { get; private set; } = string.Empty;

    public WorkflowStep? Step { get; private set; }

    private StepField() { }

    public static StepField Create(int stepId, string fieldKey, string label, string fieldType,
        string placeholder, string options, string defaultValue, string suffix,
        string helperText, bool isRequired, int displayOrder, int? maxLength,
        string validationRegex, string visibilityCondition)
    {
        if (string.IsNullOrWhiteSpace(fieldKey)) throw new ArgumentException("La clé est obligatoire.", nameof(fieldKey));

        return new StepField
        {
            StepId = stepId,
            FieldKey = fieldKey.Trim(),
            Label = label.Trim(),
            FieldType = fieldType.Trim(),
            Placeholder = placeholder,
            Options = options,
            DefaultValue = defaultValue,
            Suffix = suffix,
            HelperText = helperText,
            IsRequired = isRequired,
            DisplayOrder = displayOrder,
            IsActive = true,
            MaxLength = maxLength,
            ValidationRegex = validationRegex,
            VisibilityCondition = visibilityCondition,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string fieldKey, string label, string fieldType, string placeholder,
        string options, string defaultValue, string suffix, string helperText,
        bool isRequired, int displayOrder, bool isActive, int? maxLength,
        string validationRegex, string visibilityCondition)
    {
        FieldKey = fieldKey.Trim();
        Label = label.Trim();
        FieldType = fieldType.Trim();
        Placeholder = placeholder;
        Options = options;
        DefaultValue = defaultValue;
        Suffix = suffix;
        HelperText = helperText;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        MaxLength = maxLength;
        ValidationRegex = validationRegex;
        VisibilityCondition = visibilityCondition;
        UpdatedAt = DateTime.UtcNow;
    }
}
