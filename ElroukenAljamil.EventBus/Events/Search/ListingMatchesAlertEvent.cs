using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.Search
{
    public record ListingMatchesAlertEvent : IntegrationEvent
    {
        public Guid UserId { get; init; }
        public string UserEmail { get; init; } = string.Empty;
        public string SavedSearchName { get; init; } = string.Empty;
        public List<MatchedListingSummary> MatchedListings { get; init; } = new();
    }

    public record MatchedListingSummary
    {
        public Guid ListingId { get; init; }
        public string Title { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string? ThumbnailUrl { get; init; }
    }

}
