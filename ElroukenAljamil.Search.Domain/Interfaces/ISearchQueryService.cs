using ElroukenAljamil.Search.Domain.Entities;

namespace ElroukenAljamil.Search.Domain.Interfaces
{
    /// <summary>
    /// Interface pour les requêtes de recherche (lecture seule).
    /// </summary>
    public interface ISearchQueryService
    {
        Task<SearchResult> SearchAsync(SearchCriteria criteria, CancellationToken ct = default);
        Task<List<string>> SuggestAsync(string query, int maxSuggestions = 10, CancellationToken ct = default);
        Task<List<CategoryFacet>> GetCategoryFacetsAsync(string? query = null, CancellationToken ct = default);
        Task<PriceRange> GetPriceRangeAsync(string? category = null, CancellationToken ct = default);
    }
    /// <summary>
    /// Critères de recherche avec filtres, tri et pagination.
    /// </summary>
    public record SearchCriteria
    {
        public string? Query { get; init; }
        public string? Category { get; init; }
        public string? City { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double? RadiusKm { get; init; }
        public Guid? SellerId { get; init; }
        public string SortBy { get; init; } = "relevance";  // relevance, price_asc, price_desc, date, distance
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }

    /// <summary>
    /// Résultat de recherche paginé.
    /// </summary>
    public record SearchResult
    {
        public List<SearchableListingDocument> Items { get; init; } = new();
        public long TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
        public TimeSpan SearchDuration { get; init; }
    }

    /// <summary>
    /// Facette de catégorie (nombre d'annonces par catégorie).
    /// </summary>
    public record CategoryFacet(string Name, long Count);

    /// <summary>
    /// Fourchette de prix dans une catégorie.
    /// </summary>
    public record PriceRange(decimal Min, decimal Max, decimal Average);

}
