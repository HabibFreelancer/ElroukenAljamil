using System.Text.Json;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiDescriptionService _aiService;

        public AiController(IAiDescriptionService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("generate-description")]
        public async Task<ActionResult> GenerateDescription([FromBody] JsonElement context, CancellationToken ct)
        {
            if (context.ValueKind == JsonValueKind.Undefined || context.ValueKind == JsonValueKind.Null)
            {
                return BadRequest(new { error = "Le contexte JSON est requis pour générer la description." });
            }

            // Appel du service en propageant le CancellationToken de la requête HTTP
            var description = await _aiService.GenerateDescriptionAsync(context, ct);

            return Ok(new { description });
        }
    }
}
