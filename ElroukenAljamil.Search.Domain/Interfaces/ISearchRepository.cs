using ElroukenAljamil.Search.Domain.Entities;

namespace ElroukenAljamil.Search.Domain.Interfaces
{
    /// <summary>
    /// Interface pour les opérations de recherche dans l'index.
    /// Ne dérive pas de IRepository car Elasticsearch n'est pas une BDD relationnelle.
    /// </summary>
    public interface ISearchRepository
    {
        Task IndexAsync(SearchableListingDocument document, CancellationToken ct = default);
        Task UpdateAsync(SearchableListingDocument document, CancellationToken ct = default);
        Task DeleteAsync(Guid listingId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid listingId, CancellationToken ct = default);
    }
}
