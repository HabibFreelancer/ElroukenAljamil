using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;

namespace ElroukenAljamil.Messaging.Domain.Events
{
    public record ConversationCreatedDomainEvent(
    Guid ConversationId, Guid BuyerId, Guid SellerId,
    Guid ListingId, string ListingTitle) : BaseDomainEvent;

    public record MessageSentDomainEvent(
        Guid MessageId, Guid ConversationId,
        Guid SenderId, string SenderName,
        Guid RecipientId, string Content,
        Guid ListingId, string ListingTitle) : BaseDomainEvent;

    public record MessagesReadDomainEvent(
        Guid ConversationId, Guid UserId, int Count) : BaseDomainEvent;

    public record ConversationArchivedDomainEvent(
        Guid ConversationId, Guid UserId) : BaseDomainEvent;
}
