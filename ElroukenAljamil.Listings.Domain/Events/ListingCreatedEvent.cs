using ElroukenAljamil.Listings.Domain.Entities;


namespace ElroukenAljamil.Listings.Domain.Events
{
    public class ListingCreatedEvent : DomainEvent
    {
        public Guid ListingId { get; }
        public Guid SellerId { get; }


        public ListingCreatedEvent(Guid listingId, Guid sellerId)
        {
            ListingId = listingId;
            SellerId = sellerId;
        }
    }

}
