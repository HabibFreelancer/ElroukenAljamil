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
        sb.AppendLine("Génère une description d'annonce de vente de véhicule en français. Le format doit être :");
        sb.AppendLine("1. Une phrase d'introduction attractive (Ex: Je vends mon [Marque] [Modèle] de [Année], un [Type] spacieux avec seulement [Km] km au compteur.)");
        sb.AppendLine("2. Une liste à puces avec les caractéristiques techniques (Marque, Modèle, Année, Kilométrage, Motorisation, Carburant, Boîte de vitesses, Type, Sièges, Portes, Puissance fiscale, Date immatriculation)");
        sb.AppendLine("3. Une phrase finale : N'hésitez pas à me contacter pour plus d'informations ou pour convenir d'un essai !");
        sb.AppendLine("\nUtilise les informations suivantes :");
        
        if (context.TryGetProperty("brand", out var brand)) sb.AppendLine($"- Marque: {brand}");
        if (context.TryGetProperty("model", out var model)) sb.AppendLine($"- Modèle: {model}");
        if (context.TryGetProperty("year", out var year)) sb.AppendLine($"- Année: {year}");
        if (context.TryGetProperty("fuel", out var fuel)) sb.AppendLine($"- Carburant: {fuel}");
        if (context.TryGetProperty("gearbox", out var gearbox)) sb.AppendLine($"- Boîte: {gearbox}");
        if (context.TryGetProperty("mileage", out var mileage)) sb.AppendLine($"- Kilométrage: {mileage} km");
        if (context.TryGetProperty("fiscalPower", out var fp)) sb.AppendLine($"- Puissance fiscale: {fp} CV");
        if (context.TryGetProperty("dinPower", out var dp)) sb.AppendLine($"- Puissance DIN: {dp} Ch");
        if (context.TryGetProperty("vehicleType", out var vt)) sb.AppendLine($"- Type: {vt}");
        if (context.TryGetProperty("seats", out var seats)) sb.AppendLine($"- Sièges: {seats}");
        if (context.TryGetProperty("doors", out var doors)) sb.AppendLine($"- Portes: {doors}");
        if (context.TryGetProperty("firstCirculation", out var fc)) sb.AppendLine($"- Immatriculation: {fc}");
        if (context.TryGetProperty("color", out var color)) sb.AppendLine($"- Couleur: {color}");
        if (context.TryGetProperty("upholstery", out var uph)) sb.AppendLine($"- Sellerie: {uph}");
        if (context.TryGetProperty("equipment", out var equip)) sb.AppendLine($"- Équipements: {equip}");
        if (context.TryGetProperty("history", out var hist)) sb.AppendLine($"- Historique: {hist}");

        sb.AppendLine("\nRéponds uniquement avec la description formatée, sans commentaire.");
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

        var brand = GetValue(context, "brand") ?? "";
        var model = GetValue(context, "model") ?? "";
        var year = GetValue(context, "year") ?? "";
        var fuel = GetValue(context, "fuel") ?? "";
        var mileage = GetValue(context, "mileage") ?? "";
        var gearbox = GetValue(context, "gearbox") ?? "";
        var dinPower = GetValue(context, "dinPower") ?? "";
        var fiscalPower = GetValue(context, "fiscalPower") ?? "";
        var vehicleType = GetValue(context, "vehicleType") ?? "";
        var seats = GetValue(context, "seats") ?? "";
        var doors = GetValue(context, "doors") ?? "";
        var firstCirculation = GetValue(context, "firstCirculation") ?? "";

        // Intro
        sb.Append($"Je vends mon {brand} {model}");
        if (!string.IsNullOrEmpty(year)) sb.Append($" de {year}");
        if (!string.IsNullOrEmpty(vehicleType)) sb.Append($", un {vehicleType} spacieux et confortable");
        if (!string.IsNullOrEmpty(mileage)) sb.Append($" avec seulement {mileage} km au compteur");
        sb.AppendLine(".");

        // Details
        if (!string.IsNullOrEmpty(brand)) sb.AppendLine($"- Marque : {brand}");
        if (!string.IsNullOrEmpty(model)) sb.AppendLine($"- Mod\u00e8le : {model}");
        if (!string.IsNullOrEmpty(year)) sb.AppendLine($"- Ann\u00e9e : {year}");
        if (!string.IsNullOrEmpty(mileage)) sb.AppendLine($"- Kilom\u00e9trage : {mileage} km");
        if (!string.IsNullOrEmpty(dinPower)) sb.AppendLine($"- Motorisation : {dinPower} Ch");
        if (!string.IsNullOrEmpty(fuel)) sb.AppendLine($"- Carburant : {fuel}");
        if (!string.IsNullOrEmpty(gearbox)) sb.AppendLine($"- Bo\u00eete de vitesses : {gearbox}");
        if (!string.IsNullOrEmpty(vehicleType)) sb.AppendLine($"- Type de v\u00e9hicule : {vehicleType}");
        if (!string.IsNullOrEmpty(seats)) sb.AppendLine($"- Nombre de si\u00e8ges : {seats}");
        if (!string.IsNullOrEmpty(doors)) sb.AppendLine($"- Nombre de portes : {doors}");
        if (!string.IsNullOrEmpty(fiscalPower)) sb.AppendLine($"- Puissance fiscale : {fiscalPower} CV");
        if (!string.IsNullOrEmpty(firstCirculation)) sb.AppendLine($"- Immatriculation : {firstCirculation}");

        sb.AppendLine();
        sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");

        return sb.ToString().Trim();
    }

    private string? GetValue(JsonElement element, string key)
    {
        if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }
}
