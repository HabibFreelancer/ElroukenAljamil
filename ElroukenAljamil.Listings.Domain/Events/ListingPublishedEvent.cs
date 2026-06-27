using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Events
{
    public class ListingPublishedEvent : DomainEvent
    {
        public Guid ListingId { get; }


        public ListingPublishedEvent(Guid listingId)
        {
            ListingId = listingId;
        }
    }

}
