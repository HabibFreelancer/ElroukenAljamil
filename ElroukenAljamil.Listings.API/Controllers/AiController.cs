using System.Text.Json;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiDescriptionService _aiService;
        private readonly ILogger<AiController> _logger;

        public AiController(IAiDescriptionService aiService, ILogger<AiController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("generate-description")]
        public async Task<ActionResult> GenerateDescription([FromBody] JsonElement context, CancellationToken ct)
        {
            if (context.ValueKind == JsonValueKind.Undefined || context.ValueKind == JsonValueKind.Null)
            {
                _logger.LogWarning("Génération description IA — contexte JSON manquant");
                return BadRequest(new { error = "Le contexte JSON est requis pour générer la description." });
            }

            _logger.LogInformation("Génération description IA demandée");
            var description = await _aiService.GenerateDescriptionAsync(context, ct);
            _logger.LogInformation("Description IA générée avec succès");

            return Ok(new { description });
        }
    }
}
