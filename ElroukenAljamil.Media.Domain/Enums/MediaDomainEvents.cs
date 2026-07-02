using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Media.Domain.ValueObjects;

namespace ElroukenAljamil.Media.Domain.Enums
{
    public record MediaUploadedDomainEvent(
      Guid MediaId, Guid OwnerId, string StoragePath) : BaseDomainEvent;

    public record MediaProcessedDomainEvent(
        Guid MediaId, Guid OwnerId, MediaVariants Variants) : BaseDomainEvent;

    public record MediaProcessingFailedDomainEvent(
        Guid MediaId, Guid OwnerId, string Reason) : BaseDomainEvent;

    public record MediaAssignedToListingDomainEvent(
        Guid MediaId, Guid ListingId, Guid OwnerId) : BaseDomainEvent;

    public record MediaMarkedForDeletionDomainEvent(
        Guid MediaId, Guid OwnerId, string StoragePath, string BucketName) : BaseDomainEvent;
}
