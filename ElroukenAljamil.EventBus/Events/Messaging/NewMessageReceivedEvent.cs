using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.EventBus.Events.Messaging
{
    public record NewMessageReceivedEvent : IntegrationEvent
    {
        public Guid MessageId { get; init; }
        public Guid ConversationId { get; init; }
        public Guid SenderId { get; init; }
        public string SenderName { get; init; } = string.Empty;
        public Guid RecipientId { get; init; }
        public string RecipientEmail { get; init; } = string.Empty;
        public string MessagePreview { get; init; } = string.Empty;
        public Guid? ListingId { get; init; }
        public string? ListingTitle { get; init; }
    }
}
