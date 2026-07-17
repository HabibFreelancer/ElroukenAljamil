using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Search.Domain.Interfaces
{
    /// <summary>
    /// Service de gestion de l'index Elasticsearch (création, suppression, réindexation).
    /// </summary>
    public interface IIndexManagementService
    {
        Task CreateIndexAsync(CancellationToken ct = default);
        Task DeleteIndexAsync(CancellationToken ct = default);
        Task<bool> IndexExistsAsync(CancellationToken ct = default);
        Task ReindexAsync(CancellationToken ct = default);
        Task<IndexHealthInfo> GetHealthAsync(CancellationToken ct = default);
    }

    public record IndexHealthInfo
    {
        public string Status { get; init; } = string.Empty;
        public long DocumentCount { get; init; }
        public string IndexSize { get; init; } = string.Empty;
    }
}
