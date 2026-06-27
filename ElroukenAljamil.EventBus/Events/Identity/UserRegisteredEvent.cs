using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.EventBus.Events.Identity
{
    public record UserRegisteredEvent : IntegrationEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public string ConfirmationToken { get; init; } = string.Empty;
    }

    public record PasswordResetRequestedEvent : IntegrationEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string ResetToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
