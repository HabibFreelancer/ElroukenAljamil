using ElroukenAljamil.Search.Domain.Entities;
using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace ElroukenAljamil.Search.Infrastructure.Elasticsearch
{
    /// <summary>
    /// Service de gestion de l'index : création avec mapping, suppression, health check.
    /// </summary>
    public class ElasticsearchIndexManagementService : IIndexManagementService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchIndexManagementService> _logger;
        private readonly string _indexName;

        public ElasticsearchIndexManagementService(
            IElasticClient elasticClient,
            IConfiguration configuration,
            ILogger<ElasticsearchIndexManagementService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _indexName = configuration["Elasticsearch:DefaultIndex"] ?? "listings";
        }

        public async Task CreateIndexAsync(CancellationToken ct = default)
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(_indexName, ct: ct);
            if (existsResponse.Exists)
            {
                _logger.LogInformation("L'index '{Index}' existe déjà.", _indexName);
                return;
            }

            var createResponse = await _elasticClient.Indices.CreateAsync(_indexName, c => c
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Custom("french_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filters("lowercase", "french_stemmer", "french_stop", "asciifolding"))
                            .Custom("autocomplete_analyzer", ca => ca
                                .Tokenizer("autocomplete_tokenizer")
                                .Filters("lowercase", "asciifolding")))
                        .Tokenizers(t => t
                            .EdgeNGram("autocomplete_tokenizer", e => e
                                .MinGram(2)
                                .MaxGram(20)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)))
                        .TokenFilters(tf => tf
                            .Stemmer("french_stemmer", st => st.Language("french"))
                            .Stop("french_stop", st => st.StopWords("_french_")))))
                .Map<SearchableListingDocument>(m => m
                    .Properties(p => p
                        .Text(t => t
                            .Name(n => n.Title)
                            .Analyzer("french_analyzer")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword").IgnoreAbove(200))
                                .Completion(co => co.Name("suggest").Analyzer("autocomplete_analyzer"))))
                        .Text(t => t
                            .Name(n => n.Description)
                            .Analyzer("french_analyzer"))
                        .Keyword(k => k.Name(n => n.Category)
                            .Fields(f => f
                                .Text(tx => tx.Name("text").Analyzer("french_analyzer"))
                                .Completion(co => co.Name("suggest").Analyzer("autocomplete_analyzer"))))
                        .Text(t => t
                            .Name(n => n.City)
                            .Analyzer("french_analyzer")
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))))
                        .Number(n => n.Name(p => p.Price).Type(NumberType.ScaledFloat).ScalingFactor(100))
                        .Keyword(k => k.Name(n => n.Currency))
                        .Keyword(k => k.Name(n => n.Status))
                        .Keyword(k => k.Name(n => n.SellerId))
                        .Text(t => t.Name(n => n.SellerName))
                        .Keyword(k => k.Name(n => n.ImageUrls))
                        .Text(t => t.Name(n => n.ThumbnailUrl).Index(false))
                        .Date(d => d.Name(n => n.PublishedAt))
                        .Date(d => d.Name(n => n.UpdatedAt))
                        .GeoPoint(g => g.Name(n => n.Location)))),
                ct);

            if (!createResponse.IsValid)
            {
                _logger.LogError("Échec de la création de l'index: {Error}",
                    createResponse.OriginalException?.Message);
                throw new InvalidOperationException(
                    $"Impossible de créer l'index: {createResponse.ServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Index '{Index}' créé avec succès.", _indexName);
        }

        public async Task DeleteIndexAsync(CancellationToken ct = default)
        {
            var response = await _elasticClient.Indices.DeleteAsync(_indexName, ct: ct);

            if (!response.IsValid)
                _logger.LogWarning("Échec de la suppression de l'index '{Index}'.", _indexName);
            else
                _logger.LogInformation("Index '{Index}' supprimé.", _indexName);
        }

        public async Task<bool> IndexExistsAsync(CancellationToken ct = default)
        {
            var response = await _elasticClient.Indices.ExistsAsync(_indexName, ct: ct);
            return response.Exists;
        }

        public async Task ReindexAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Réindexation de '{Index}'...", _indexName);
            await DeleteIndexAsync(ct);
            await CreateIndexAsync(ct);
            _logger.LogInformation("Réindexation terminée. L'index est vide, les événements re-rempliront les données.");
        }

        public async Task<IndexHealthInfo> GetHealthAsync(CancellationToken ct = default)
        {
            var statsResponse = await _elasticClient.Indices.StatsAsync(_indexName, ct: ct);

            if (!statsResponse.IsValid)
            {
                return new IndexHealthInfo
                {
                    Status = "unavailable",
                    DocumentCount = 0,
                    IndexSize = "0"
                };
            }

            var indexStats = statsResponse.Indices.ContainsKey(_indexName)
                ? statsResponse.Indices[_indexName]
                : null;

            return new IndexHealthInfo
            {
                Status = "healthy",
                DocumentCount = indexStats?.Total?.Documents?.Count ?? 0,
                IndexSize = indexStats?.Total?.Store?.SizeInBytes.ToString() ?? "0"
            };
        }
    }
}
