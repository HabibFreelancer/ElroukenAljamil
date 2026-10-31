namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.BugReport;

public record BugReportCreatedEvent : IntegrationEvent
{
    public int BugReportId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string SeverityLabel { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Module { get; init; }
    public string ReportedByUserId { get; init; } = string.Empty;
    public string ReportedByUserName { get; init; } = string.Empty;
}

public record BugStatusChangedEvent : IntegrationEvent
{
    public int BugReportId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string OldStatusLabel { get; init; } = string.Empty;
    public string NewStatusLabel { get; init; } = string.Empty;
    public string ReportedByUserId { get; init; } = string.Empty;
    public string ChangedByUserId { get; init; } = string.Empty;
}
