using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Application.DTOs
{
    public record NotificationDto
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Channel { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? Metadata { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ReadAt { get; init; }
    }
}
