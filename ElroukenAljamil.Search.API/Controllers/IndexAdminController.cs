using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Search.API.Controllers
{
    /// <summary>
    /// Endpoints d'administration de l'index (réservés aux admins).
    /// </summary>
    [ApiController]
    [Route("api/search/admin")]
    [Authorize(Roles = "Admin")]
    public class IndexAdminController : ControllerBase
    {
        private readonly IIndexManagementService _indexManagement;

        public IndexAdminController(IIndexManagementService indexManagement)
        {
            _indexManagement = indexManagement;
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
            await _indexManagement.ReindexAsync(ct);
            return Ok(new { message = "Index recréé. Les données seront re-populées via les événements." });
        }
    }
}
