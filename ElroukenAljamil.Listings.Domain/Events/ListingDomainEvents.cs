using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Listings.Domain.ValueObjects;

namespace ElroukenAljamil.Listings.Domain.Events
{
    public record ListingCreatedDomainEvent(Guid ListingId, string Title, Guid SellerId) : BaseDomainEvent;

    public record ListingUpdatedDomainEvent(Guid ListingId, string Title, Guid SellerId) : BaseDomainEvent;

    public record ListingDeactivatedDomainEvent(Guid ListingId, Guid SellerId) : BaseDomainEvent;

    public record ListingSoldDomainEvent(Guid ListingId, Guid SellerId) : BaseDomainEvent;

    public record ListingExpiredDomainEvent(Guid ListingId, Guid SellerId) : BaseDomainEvent;
}
