using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Search.API.Controllers
{
    [ApiController]
    [Route("api/search/admin")]
    [Authorize(Roles = "Admin")]
    public class IndexAdminController : ControllerBase
    {
        private readonly IIndexManagementService _indexManagement;
        private readonly ILogger<IndexAdminController> _logger;

        public IndexAdminController(IIndexManagementService indexManagement, ILogger<IndexAdminController> logger)
        {
            _indexManagement = indexManagement;
            _logger = logger;
        }

        /// <summary>
        /// État de santé de l'index Elasticsearch.
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(typeof(IndexHealthInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHealth(CancellationToken ct)
        {
            var health = await _indexManagement.GetHealthAsync(ct);
            return Ok(health);
        }

        /// <summary>
        /// Recrée l'index (supprime et recrée avec le nouveau mapping).
        /// ⚠️ Supprime toutes les données de l'index !
        /// </summary>
        [HttpPost("reindex")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Reindex(CancellationToken ct)
        {
            _logger.LogWarning("Reindex demandé par admin — suppression et recréation de l'index Elasticsearch");
            await _indexManagement.ReindexAsync(ct);
            _logger.LogInformation("Reindex terminé avec succès");
            return Ok(new { message = "Index recréé. Les données seront re-populées via les événements." });
        }
    }
}
