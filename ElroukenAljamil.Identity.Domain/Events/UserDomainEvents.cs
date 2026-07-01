using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Identity.Domain.Events
{

    public record UserRegisteredDomainEvent(Guid UserId, string Email, string UserName) : BaseDomainEvent;

    public record UserEmailVerifiedDomainEvent(Guid UserId, string Email) : BaseDomainEvent;

    public record UserLockedOutDomainEvent(Guid UserId, string Email, DateTime LockedUntil) : BaseDomainEvent;

    public record UserPromotedToSellerDomainEvent(Guid UserId, string Email) : BaseDomainEvent;

    public record UserDeactivatedDomainEvent(Guid UserId, string Email) : BaseDomainEvent;
}
