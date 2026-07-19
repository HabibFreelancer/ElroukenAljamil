namespace ElroukenAljamil.Notification.Application.DTOs
{
    public record NotificationPreferenceDto
    {
        public string NotificationType { get; init; } = string.Empty;
        public bool EmailEnabled { get; init; }
        public bool SmsEnabled { get; init; }
        public bool PushEnabled { get; init; }
        public bool InAppEnabled { get; init; }
    }

    public record UpdatePreferenceRequest
    {
        public string NotificationType { get; init; } = string.Empty;
        public bool EmailEnabled { get; init; }
        public bool SmsEnabled { get; init; }
        public bool PushEnabled { get; init; }
        public bool InAppEnabled { get; init; }
    }
}
