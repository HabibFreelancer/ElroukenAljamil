using ElroukenAljamil.BuildingBlocks.EventBus.Events.BugReport;
using ElroukenAljamil.Notification.Application.Services;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MassTransit;

namespace ElroukenAljamil.Notification.Application.EventConsumers;

public class BugReportCreatedConsumer : IConsumer<BugReportCreatedEvent>
{
    private readonly INotificationOrchestrator _orchestrator;
    private readonly IRecipientResolver _resolver;

    public BugReportCreatedConsumer(INotificationOrchestrator orchestrator, IRecipientResolver resolver)
    {
        _orchestrator = orchestrator;
        _resolver = resolver;
    }

    public async Task Consume(ConsumeContext<BugReportCreatedEvent> context)
    {
        var e = context.Message;
        var ct = context.CancellationToken;

        // Notifier tous les admins
        var adminIds = await _resolver.GetAdminUserIdsAsync(ct);

        var templateData = new Dictionary<string, object>
        {
            ["bugId"]       = e.BugReportId,
            ["title"]       = e.Title,
            ["severity"]    = e.SeverityLabel,
            ["category"]    = e.Category,
            ["module"]      = e.Module ?? "—",
            ["reportedBy"]  = e.ReportedByUserName,
            ["url"]         = $"/recette"
        };

        foreach (var adminId in adminIds)
        {
            try
            {
                await _orchestrator.OrchestrateAsync(
                    adminId, NotificationType.BugReportCreated, "fr", templateData, ct);
            }
            catch { /* ne pas bloquer les autres admins */ }
        }

        // Notifier le rapporteur (confirmation in-app)
        if (Guid.TryParse(e.ReportedByUserId, out var reporterId))
        {
            try
            {
                await _orchestrator.OrchestrateAsync(
                    reporterId, NotificationType.BugReportCreated, "fr", templateData, ct);
            }
            catch { }
        }
    }
}
