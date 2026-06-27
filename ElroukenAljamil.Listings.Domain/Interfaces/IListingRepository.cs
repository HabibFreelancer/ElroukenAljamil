using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    /// <summary>
    /// Contrat du repository - défini dans le domaine, implémenté dans l'infrastructure.
    /// </summary>
    public interface IListingRepository
    {
        Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Listing>> GetActiveByCategoryAsync(Guid categoryId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(Listing listing, CancellationToken cancellationToken = default);
        Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

}
