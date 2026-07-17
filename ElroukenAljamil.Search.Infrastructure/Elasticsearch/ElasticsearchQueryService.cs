using ElroukenAljamil.Search.Domain.Entities;
using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;

namespace ElroukenAljamil.Search.Infrastructure.Elasticsearch
{
    /// <summary>
    /// Service de requêtes de recherche via Elasticsearch.
    /// Supporte : full-text, filtres, géolocalisation, tri, pagination, suggestions, facettes.
    /// </summary>
    public class ElasticsearchQueryService : ISearchQueryService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchQueryService> _logger;

        public ElasticsearchQueryService(IElasticClient elasticClient, ILogger<ElasticsearchQueryService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<SearchResult> SearchAsync(SearchCriteria criteria, CancellationToken ct = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<SearchableListingDocument>(s =>
            {
                s.From((criteria.Page - 1) * criteria.PageSize)
                 .Size(criteria.PageSize);

                // Construction de la requête Bool
                s.Query(q => q.Bool(b =>
                {
                    var must = new List<Func<QueryContainerDescriptor<SearchableListingDocument>, QueryContainer>>();
                    var filter = new List<Func<QueryContainerDescriptor<SearchableListingDocument>, QueryContainer>>();

                    // Full-text search (multi-match avec boosting)
                    if (!string.IsNullOrWhiteSpace(criteria.Query))
                    {
                        must.Add(m => m.MultiMatch(mm => mm
                            .Query(criteria.Query)
                            .Fields(f => f
                                .Field(p => p.Title, boost: 3)
                                .Field(p => p.Description, boost: 1)
                                .Field(p => p.Category, boost: 2)
                                .Field(p => p.City, boost: 1.5))
                            .Type(TextQueryType.BestFields)
                            .Fuzziness(Fuzziness.Auto)
                            .MinimumShouldMatch("75%")));
                    }

                    // Filtre par catégorie
                    if (!string.IsNullOrEmpty(criteria.Category))
                    {
                        filter.Add(f => f.Term(t => t
                            .Field(p => p.Category.Suffix("keyword"))
                            .Value(criteria.Category)));
                    }

                    // Filtre par ville
                    if (!string.IsNullOrEmpty(criteria.City))
                    {
                        filter.Add(f => f.Match(m => m
                            .Field(p => p.City)
                            .Query(criteria.City)));
                    }

                    // Filtre par prix
                    if (criteria.MinPrice.HasValue || criteria.MaxPrice.HasValue)
                    {
                        filter.Add(f => f.Range(r =>
                        {
                            var rangeQuery = r.Field(p => p.Price);
                            if (criteria.MinPrice.HasValue)
                                rangeQuery.GreaterThanOrEquals((double)criteria.MinPrice.Value);
                            if (criteria.MaxPrice.HasValue)
                                rangeQuery.LessThanOrEquals((double)criteria.MaxPrice.Value);
                            return rangeQuery;
                        }));
                    }

                    // Filtre par vendeur
                    if (criteria.SellerId.HasValue)
                    {
                        filter.Add(f => f.Term(t => t
                            .Field(p => p.SellerId)
                            .Value(criteria.SellerId.Value)));
                    }

                    // Filtre géographique (rayon)
                    if (criteria.Latitude.HasValue && criteria.Longitude.HasValue && criteria.RadiusKm.HasValue)
                    {
                        filter.Add(f => f.GeoDistance(g => g
                            .Field(p => p.Location)
                            .Distance($"{criteria.RadiusKm}km")
                            .Location(criteria.Latitude.Value, criteria.Longitude.Value)));
                    }

                    // Toujours filtrer sur les annonces actives
                    filter.Add(f => f.Term(t => t
                        .Field(p => p.Status.Suffix("keyword"))
                        .Value("Active")));

                    b.Must(must.ToArray());
                    b.Filter(filter.ToArray());

                    return b;
                }));

                // Tri
                ApplySorting(s, criteria);

                return s;
            }, ct);

            if (!searchResponse.IsValid)
            {
                _logger.LogError("Erreur de recherche Elasticsearch: {Error}",
                    searchResponse.OriginalException?.Message);
                return new SearchResult { Items = new(), TotalCount = 0, Page = criteria.Page, PageSize = criteria.PageSize };
            }

            return new SearchResult
            {
                Items = searchResponse.Documents.ToList(),
                TotalCount = searchResponse.Total,
                Page = criteria.Page,
                PageSize = criteria.PageSize,
                SearchDuration = TimeSpan.FromMilliseconds(searchResponse.Took)
            };
        }

        public async Task<List<string>> SuggestAsync(string query, int maxSuggestions = 10, CancellationToken ct = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<SearchableListingDocument>(s => s
                .Size(0)
                .Suggest(su => su
                    .Completion("title-suggest", cs => cs
                        .Field(f => f.Title.Suffix("suggest"))
                        .Prefix(query)
                        .Size(maxSuggestions)
                        .Fuzzy(f => f.Fuzziness(Fuzziness.Auto)))
                    .Completion("category-suggest", cs => cs
                        .Field(f => f.Category.Suffix("suggest"))
                        .Prefix(query)
                        .Size(5))), ct);

            var suggestions = new List<string>();

            if (searchResponse.IsValid && searchResponse.Suggest != null)
            {
                if (searchResponse.Suggest.ContainsKey("title-suggest"))
                {
                    suggestions.AddRange(searchResponse.Suggest["title-suggest"]
                        .SelectMany(s => s.Options)
                        .Select(o => o.Text));
                }

                if (searchResponse.Suggest.ContainsKey("category-suggest"))
                {
                    suggestions.AddRange(searchResponse.Suggest["category-suggest"]
                        .SelectMany(s => s.Options)
                        .Select(o => o.Text));
                }
            }

            return suggestions.Distinct().Take(maxSuggestions).ToList();
        }

        public async Task<List<CategoryFacet>> GetCategoryFacetsAsync(string? query = null, CancellationToken ct = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<SearchableListingDocument>(s =>
            {
                s.Size(0)
                 .Query(q =>
                 {
                     if (!string.IsNullOrWhiteSpace(query))
                     {
                         return q.MultiMatch(mm => mm
                             .Query(query)
                             .Fields(f => f.Field(p => p.Title).Field(p => p.Description)));
                     }
                     return q.Term(t => t.Field(p => p.Status.Suffix("keyword")).Value("Active"));
                 })
                 .Aggregations(a => a
                     .Terms("categories", t => t
                         .Field(f => f.Category.Suffix("keyword"))
                         .Size(50)));

                return s;
            }, ct);

            var facets = new List<CategoryFacet>();

            if (searchResponse.IsValid)
            {
                var categoryAgg = searchResponse.Aggregations.Terms("categories");
                if (categoryAgg != null)
                {
                    facets.AddRange(categoryAgg.Buckets
                        .Select(b => new CategoryFacet(b.Key, b.DocCount ?? 0)));
                }
            }

            return facets;
        }

        public async Task<PriceRange> GetPriceRangeAsync(string? category = null, CancellationToken ct = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<SearchableListingDocument>(s =>
            {
                s.Size(0)
                 .Query(q =>
                 {
                     var mustFilters = new List<Func<QueryContainerDescriptor<SearchableListingDocument>, QueryContainer>>
                     {
                     f => f.Term(t => t.Field(p => p.Status.Suffix("keyword")).Value("Active"))
                     };

                     if (!string.IsNullOrEmpty(category))
                     {
                         mustFilters.Add(f => f.Term(t => t.Field(p => p.Category.Suffix("keyword")).Value(category)));
                     }

                     return q.Bool(b => b.Filter(mustFilters.ToArray()));
                 })
                 .Aggregations(a => a
                     .Stats("price_stats", st => st.Field(f => f.Price)));

                return s;
            }, ct);

            if (searchResponse.IsValid)
            {
                var priceStats = searchResponse.Aggregations.Stats("price_stats");
                if (priceStats != null && priceStats.Count > 0)
                {
                    return new PriceRange(
                        (decimal)(priceStats.Min ?? 0),
                        (decimal)(priceStats.Max ?? 0),
                        (decimal)(priceStats.Average ?? 0));
                }
            }

            return new PriceRange(0, 0, 0);
        }

        private static void ApplySorting(
            SearchDescriptor<SearchableListingDocument> descriptor,
            SearchCriteria criteria)
        {
            switch (criteria.SortBy.ToLowerInvariant())
            {
                case "price_asc":
                    descriptor.Sort(s => s.Ascending(p => p.Price));
                    break;
                case "price_desc":
                    descriptor.Sort(s => s.Descending(p => p.Price));
                    break;
                case "date":
                    descriptor.Sort(s => s.Descending(p => p.PublishedAt));
                    break;
                case "distance" when criteria.Latitude.HasValue && criteria.Longitude.HasValue:
                    descriptor.Sort(s => s.GeoDistance(g => g
                        .Field(p => p.Location)
                        .Points(new Nest.GeoLocation(criteria.Latitude.Value, criteria.Longitude.Value))
                        .Order(SortOrder.Ascending)
                        .Unit(DistanceUnit.Kilometers)));
                    break;
                default: // "relevance" — Elasticsearch utilise le score par défaut
                    descriptor.Sort(s => s.Descending(SortSpecialField.Score));
                    break;
            }
        }
    }
}
