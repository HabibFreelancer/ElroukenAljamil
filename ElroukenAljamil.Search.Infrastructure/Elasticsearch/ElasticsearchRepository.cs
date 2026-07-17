using ElroukenAljamil.Search.Domain.Entities;
using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Nest;

namespace ElroukenAljamil.Search.Infrastructure.Elasticsearch
{
    /// <summary>
    /// Repository pour les opérations d'écriture dans l'index Elasticsearch.
    /// </summary>
    public class ElasticsearchRepository : ISearchRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchRepository> _logger;

        public ElasticsearchRepository(IElasticClient elasticClient, ILogger<ElasticsearchRepository> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task IndexAsync(SearchableListingDocument document, CancellationToken ct = default)
        {
            var response = await _elasticClient.IndexDocumentAsync(document, ct);

            if (!response.IsValid)
            {
                _logger.LogError("Échec de l'indexation du document {Id}: {Error}",
                    document.Id, response.OriginalException?.Message);
                throw new InvalidOperationException(
                    $"Impossible d'indexer le document {document.Id}: {response.ServerError?.Error?.Reason}");
            }

            _logger.LogDebug("Document {Id} indexé avec succès.", document.Id);
        }

        public async Task UpdateAsync(SearchableListingDocument document, CancellationToken ct = default)
        {
            var response = await _elasticClient.UpdateAsync<SearchableListingDocument>(
                document.Id, u => u.Doc(document).DocAsUpsert(), ct);

            if (!response.IsValid)
            {
                _logger.LogError("Échec de la mise à jour du document {Id}: {Error}",
                    document.Id, response.OriginalException?.Message);
                throw new InvalidOperationException(
                    $"Impossible de mettre à jour le document {document.Id}: {response.ServerError?.Error?.Reason}");
            }

            _logger.LogDebug("Document {Id} mis à jour.", document.Id);
        }

        public async Task DeleteAsync(Guid listingId, CancellationToken ct = default)
        {
            var response = await _elasticClient.DeleteAsync<SearchableListingDocument>(listingId, d => d, ct);

            if (!response.IsValid && response.Result != Result.NotFound)
            {
                _logger.LogError("Échec de la suppression du document {Id}: {Error}",
                    listingId, response.OriginalException?.Message);
                throw new InvalidOperationException(
                    $"Impossible de supprimer le document {listingId}: {response.ServerError?.Error?.Reason}");
            }

            _logger.LogDebug("Document {Id} supprimé de l'index.", listingId);
        }

        public async Task<bool> ExistsAsync(Guid listingId, CancellationToken ct = default)
        {
            var response = await _elasticClient.DocumentExistsAsync<SearchableListingDocument>(listingId, d => d, ct);
            return response.Exists;
        }
    }
}
