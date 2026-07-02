using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Media.Domain.Entities;
using ElroukenAljamil.Media.Domain.Enums;

namespace ElroukenAljamil.Media.Domain.Interfaces
{
    public interface IMediaFileRepository : IRepository<MediaFile>
    {
        Task<IReadOnlyList<MediaFile>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default);
        Task<IReadOnlyList<MediaFile>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default);
        Task<IReadOnlyList<MediaFile>> GetByStatusAsync(MediaStatus status, CancellationToken ct = default);
        Task<IReadOnlyList<MediaFile>> GetOrphanedFilesAsync(TimeSpan olderThan, CancellationToken ct = default);
        Task<int> GetCountByListingAsync(Guid listingId, CancellationToken ct = default);
        Task<int> GetCountByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    }
}
