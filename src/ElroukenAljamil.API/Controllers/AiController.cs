using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AiController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("generate-description")]
    public async Task<ActionResult> GenerateDescription([FromBody] JsonElement context)
    {
        var prompt = BuildPrompt(context);

        // Try Hugging Face Inference API (free)
        var hfToken = _configuration["HuggingFace:ApiToken"] ?? "";
        
        if (!string.IsNullOrEmpty(hfToken))
        {
            try
            {
                var description = await CallHuggingFace(prompt, hfToken);
                if (!string.IsNullOrWhiteSpace(description))
                    return Ok(new { description });
            }
            catch { /* Fallback below */ }
        }

        // Fallback: generate a template-based description
        var fallback = GenerateFallbackDescription(context);
        return Ok(new { description = fallback });
    }

    private string BuildPrompt(JsonElement context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Génère une description d'annonce de vente de véhicule en français, professionnelle et attractive, en 4-5 phrases. Utilise les informations suivantes :");
        
        if (context.TryGetProperty("brand", out var brand)) sb.AppendLine($"- Marque: {brand}");
        if (context.TryGetProperty("model", out var model)) sb.AppendLine($"- Modèle: {model}");
        if (context.TryGetProperty("year", out var year)) sb.AppendLine($"- Année: {year}");
        if (context.TryGetProperty("fuel", out var fuel)) sb.AppendLine($"- Énergie: {fuel}");
        if (context.TryGetProperty("gearbox", out var gearbox)) sb.AppendLine($"- Boîte: {gearbox}");
        if (context.TryGetProperty("mileage", out var mileage)) sb.AppendLine($"- Kilométrage: {mileage} km");
        if (context.TryGetProperty("fiscalPower", out var fp)) sb.AppendLine($"- Puissance fiscale: {fp} CV");
        if (context.TryGetProperty("dinPower", out var dp)) sb.AppendLine($"- Puissance DIN: {dp} Ch");
        if (context.TryGetProperty("color", out var color)) sb.AppendLine($"- Couleur: {color}");
        if (context.TryGetProperty("vehicleState", out var state)) sb.AppendLine($"- État: {state}");
        if (context.TryGetProperty("title", out var title)) sb.AppendLine($"- Titre annonce: {title}");

        sb.AppendLine("\nRéponds uniquement avec la description, sans titre ni commentaire.");
        return sb.ToString();
    }

    private async Task<string?> CallHuggingFace(string prompt, string token)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Using a free model on Hugging Face
        var url = "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.2";

        var payload = new
        {
            inputs = $"<s>[INST] {prompt} [/INST]",
            parameters = new { max_new_tokens = 300, temperature = 0.7 }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            // HF returns array of objects with "generated_text"
            if (result.ValueKind == JsonValueKind.Array && result.GetArrayLength() > 0)
            {
                var generated = result[0].GetProperty("generated_text").GetString() ?? "";
                // Remove the prompt from the response
                var instEnd = generated.LastIndexOf("[/INST]");
                if (instEnd >= 0) generated = generated.Substring(instEnd + 7).Trim();
                return generated;
            }
        }

        return null;
    }

    private string GenerateFallbackDescription(JsonElement context)
    {
        var sb = new StringBuilder();

        var brand = GetValue(context, "brand");
        var model = GetValue(context, "model");
        var year = GetValue(context, "year");
        var fuel = GetValue(context, "fuel");
        var mileage = GetValue(context, "mileage");
        var gearbox = GetValue(context, "gearbox");
        var state = GetValue(context, "vehicleState");

        if (!string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(model))
            sb.AppendLine($"À vendre : {brand} {model} {year}.".Trim());

        if (!string.IsNullOrEmpty(fuel) || !string.IsNullOrEmpty(gearbox))
            sb.AppendLine($"Motorisation {fuel}, boîte {gearbox}.");

        if (!string.IsNullOrEmpty(mileage))
            sb.AppendLine($"Kilométrage : {mileage} km.");

        if (!string.IsNullOrEmpty(state))
            sb.AppendLine($"Véhicule en {state}.");

        sb.AppendLine("Entretien suivi, carnet à jour. Aucun frais à prévoir.");
        sb.AppendLine("N'hésitez pas à me contacter pour plus d'informations ou pour organiser un essai.");

        return sb.ToString().Trim();
    }

    private string? GetValue(JsonElement element, string key)
    {
        if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }
}
