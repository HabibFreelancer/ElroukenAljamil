using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;

namespace ElroukenAljamil.Listings.Application.Interfaces
{
    public interface IListingRepository : IRepository<Listing>
    {
        Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default);
        Task<IReadOnlyList<Listing>> GetByCategoryAsync(string category, CancellationToken ct = default);
        Task<IReadOnlyList<Listing>> GetByStatusAsync(ListingStatus status, CancellationToken ct = default);
        Task<(IReadOnlyList<Listing> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            ListingStatus? status = null,
            string? category = null,
            Guid? sellerId = null,
            CancellationToken ct = default);
        Task<int> GetCountBySellerAsync(Guid sellerId, CancellationToken ct = default);
        Task<IReadOnlyList<Listing>> GetExpiredListingsAsync(CancellationToken ct = default);
    }
}
