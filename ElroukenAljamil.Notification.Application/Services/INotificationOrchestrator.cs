using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Application.Services
{
    public interface INotificationOrchestrator
    {
        Task OrchestrateAsync(
            Guid recipientId,
            NotificationType type,
            string language,
            Dictionary<string, object> templateData,
            CancellationToken ct = default);
    }
}
