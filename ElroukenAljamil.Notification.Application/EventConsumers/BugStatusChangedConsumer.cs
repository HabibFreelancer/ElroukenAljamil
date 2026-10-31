using ElroukenAljamil.BuildingBlocks.EventBus.Events.BugReport;
using ElroukenAljamil.Notification.Application.Services;
using ElroukenAljamil.Notification.Domain.Enums;
using MassTransit;

namespace ElroukenAljamil.Notification.Application.EventConsumers;

public class BugStatusChangedConsumer : IConsumer<BugStatusChangedEvent>
{
    private readonly INotificationOrchestrator _orchestrator;

    public BugStatusChangedConsumer(INotificationOrchestrator orchestrator)
        => _orchestrator = orchestrator;

    public async Task Consume(ConsumeContext<BugStatusChangedEvent> context)
    {
        var e = context.Message;
        var ct = context.CancellationToken;

        if (!Guid.TryParse(e.ReportedByUserId, out var reporterId))
            return;

        var templateData = new Dictionary<string, object>
        {
            ["bugId"]     = e.BugReportId,
            ["title"]     = e.Title,
            ["oldStatus"] = e.OldStatusLabel,
            ["newStatus"] = e.NewStatusLabel,
            ["url"]       = $"/recette"
        };

        try
        {
            await _orchestrator.OrchestrateAsync(
                reporterId, NotificationType.BugStatusChanged, "fr", templateData, ct);
        }
        catch { }
    }
}
