using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Application.DTOs
{
    public record SendNotificationRequest
    {
        public Guid RecipientId { get; init; }
        public NotificationType Type { get; init; }
        public string Language { get; init; } = "fr";
        public Dictionary<string, object> TemplateData { get; init; } = new();
    }
}
