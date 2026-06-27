using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.EventBus.Events.Media
{
    public record MediaProcessingCompletedEvent : IntegrationEvent
    {
        public Guid MediaId { get; init; }
        public Guid OwnerId { get; init; }
        public Guid? ListingId { get; init; }
        public string OriginalUrl { get; init; } = string.Empty;
        public string ThumbnailUrl { get; init; } = string.Empty;
        public string MediumUrl { get; init; } = string.Empty;
        public string LargeUrl { get; init; } = string.Empty;
    }

    public record MediaProcessingFailedEvent : IntegrationEvent
    {
        public Guid MediaId { get; init; }
        public Guid OwnerId { get; init; }
        public string FileName { get; init; } = string.Empty;
        public string ErrorMessage { get; init; } = string.Empty;
    }
}
