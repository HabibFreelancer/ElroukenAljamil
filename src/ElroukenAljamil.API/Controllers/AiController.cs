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
        var category = GetValue(context, "category") ?? "";
        var isMoto = category.ToLower().Contains("moto") || (GetValue(context, "cylindree") != null) || (GetValue(context, "motoType") != null);
        var isCaravan = category.ToLower().Contains("caravan");
        var isUtilitaire = category.ToLower().Contains("utilitaire");
        var isNautisme = category.ToLower().Contains("nautis");
        var isEquipement = category.ToLower().Contains("quipement");

        if (isMoto)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de moto en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends ma Yamaha MT-07 de 2022, une roadster agile et puissante avec seulement 5 000 km au compteur.");
            sb.AppendLine("- Marque : Yamaha");
            sb.AppendLine("- Mod\u00e8le : MT-07");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Kilom\u00e9trage : 5 000 km");
            sb.AppendLine("- Cylindr\u00e9e : 600 - 900 cm\u00b3");
            sb.AppendLine("- Type : Roadster");
            sb.AppendLine("- Couleur : Noir");
            sb.AppendLine("- \u00c9quipements : ABS, D\u00e9marreur \u00e9lectrique, Carnet d'entretien");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");
        }
        else if (isCaravan)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de camping-car ou caravane en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon camping-car Chausson Flash de 2020, un profil\u00e9 spacieux et bien \u00e9quip\u00e9 avec 25 000 km au compteur.");
            sb.AppendLine("- Type : Camping-car semi int\u00e9gr\u00e9");
            sb.AppendLine("- Mod\u00e8le : Chausson Flash");
            sb.AppendLine("- Ann\u00e9e : 2020");
            sb.AppendLine("- Kilom\u00e9trage : 25 000 km");
            sb.AppendLine("- Couchages : 4 personnes");
            sb.AppendLine("- \u00c9tat : Tr\u00e8s bon \u00e9tat");
            sb.AppendLine("Id\u00e9al pour les vacances en famille. V\u00e9hicule entretenu r\u00e9guli\u00e8rement.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour organiser une visite !");
        }
        else if (isUtilitaire)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de v\u00e9hicule utilitaire en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Renault Master L2H2 de 2021, un fourgon fiable et spacieux avec seulement 45 000 km.");
            sb.AppendLine("- Marque : Renault");
            sb.AppendLine("- Mod\u00e8le : Master");
            sb.AppendLine("- Ann\u00e9e : 2021");
            sb.AppendLine("- Kilom\u00e9trage : 45 000 km");
            sb.AppendLine("- Carburant : Diesel");
            sb.AppendLine("- Version : L2H2");
            sb.AppendLine("- Volume : 10 m\u00b3");
            sb.AppendLine("- PTAC : 3,5 t");
            sb.AppendLine("- TVA r\u00e9cup\u00e9rable : Oui");
            sb.AppendLine("V\u00e9hicule id\u00e9al pour professionnels. Entretien suivi en concession.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else if (isNautisme)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de bateau ou v\u00e9hicule nautique en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Jet Ski Yamaha VX de 2022, en excellent \u00e9tat avec seulement 50 heures de navigation.");
            sb.AppendLine("- Type : Jet Ski");
            sb.AppendLine("- Marque : Yamaha");
            sb.AppendLine("- Mod\u00e8le : VX");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Heures : 50h");
            sb.AppendLine("- \u00c9tat : Comme neuf");
            sb.AppendLine("Remorque incluse. Id\u00e9al pour la saison estivale.");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else if (isEquipement)
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente d'\u00e9quipement ou pi\u00e8ce d\u00e9tach\u00e9e en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends un jeu de 4 jantes aluminium 17 pouces pour BMW S\u00e9rie 3, en tr\u00e8s bon \u00e9tat.");
            sb.AppendLine("- Type : Pneus & jantes");
            sb.AppendLine("- Compatibilit\u00e9 : BMW S\u00e9rie 3 (E90/F30)");
            sb.AppendLine("- Dimensions : 17 pouces");
            sb.AppendLine("- \u00c9tat : Tr\u00e8s bon \u00e9tat, pas de voile");
            sb.AppendLine("- Pneus inclus : Oui (Michelin Pilot Sport 225/45 R17)");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations !");
        }
        else
        {
            sb.AppendLine("G\u00e9n\u00e8re une description d'annonce de vente de v\u00e9hicule en fran\u00e7ais.");
            sb.AppendLine("Voici un exemple du format attendu :");
            sb.AppendLine("");
            sb.AppendLine("Je vends mon Citro\u00ebn C5 Aircross de 2022, un SUV spacieux et confortable avec seulement 4 564 km au compteur.");
            sb.AppendLine("- Marque : Citro\u00ebn");
            sb.AppendLine("- Mod\u00e8le : C5 Aircross");
            sb.AppendLine("- Ann\u00e9e : 2022");
            sb.AppendLine("- Kilom\u00e9trage : 4 564 km");
            sb.AppendLine("- Motorisation : 130 Ch");
            sb.AppendLine("- Carburant : Essence");
            sb.AppendLine("- Bo\u00eete de vitesses : Automatique");
            sb.AppendLine("- Couleur : Rose");
            sb.AppendLine("- Type de v\u00e9hicule : SUV");
            sb.AppendLine("- Nombre de si\u00e8ges : 5");
            sb.AppendLine("- Puissance fiscale : 7 Cv");
            sb.AppendLine("- Contr\u00f4le technique : Valide jusqu'en 03/2027");
            sb.AppendLine("N'h\u00e9sitez pas \u00e0 me contacter pour plus d'informations ou pour convenir d'un essai !");
        }

        sb.AppendLine("");
        sb.AppendLine("Maintenant g\u00e9n\u00e8re une description EXACTEMENT dans ce format avec les informations suivantes :");
        
        if (context.TryGetProperty("brand", out var brand)) sb.AppendLine($"- Marque: {brand}");
        if (context.TryGetProperty("model", out var model)) sb.AppendLine($"- Mod\u00e8le: {model}");
        if (context.TryGetProperty("year", out var year)) sb.AppendLine($"- Ann\u00e9e: {year}");
        if (context.TryGetProperty("fuel", out var fuel)) sb.AppendLine($"- Carburant: {fuel}");
        if (context.TryGetProperty("gearbox", out var gearbox)) sb.AppendLine($"- Bo\u00eete: {gearbox}");
        if (context.TryGetProperty("mileage", out var mileage)) sb.AppendLine($"- Kilom\u00e9trage: {mileage} km");
        if (context.TryGetProperty("cylindree", out var cyl)) sb.AppendLine($"- Cylindr\u00e9e: {cyl}");
        if (context.TryGetProperty("motoType", out var mt)) sb.AppendLine($"- Type de moto: {mt}");
        if (context.TryGetProperty("vehicleType", out var vt)) sb.AppendLine($"- Type: {vt}");
        if (context.TryGetProperty("fiscalPower", out var fp)) sb.AppendLine($"- Puissance fiscale: {fp} CV");
        if (context.TryGetProperty("dinPower", out var dp)) sb.AppendLine($"- Motorisation: {dp} Ch");
        if (context.TryGetProperty("seats", out var seats)) sb.AppendLine($"- Si\u00e8ges: {seats}");
        if (context.TryGetProperty("doors", out var doors)) sb.AppendLine($"- Portes: {doors}");
        if (context.TryGetProperty("color", out var color)) sb.AppendLine($"- Couleur: {color}");
        if (context.TryGetProperty("technicalControl", out var tc)) sb.AppendLine($"- Contr\u00f4le technique valide jusqu'en: {tc}");
        if (context.TryGetProperty("upholstery", out var uph)) sb.AppendLine($"- Sellerie: {uph}");
        if (context.TryGetProperty("equipment", out var equip)) sb.AppendLine($"- \u00c9quipements: {equip}");
        if (context.TryGetProperty("equipmentType", out var eqType)) sb.AppendLine($"- Type d'\u00e9quipement: {eqType}");
        if (context.TryGetProperty("history", out var hist)) sb.AppendLine($"- Historique: {hist}");
        if (context.TryGetProperty("license", out var lic)) sb.AppendLine($"- Permis: {lic}");
        if (context.TryGetProperty("volume", out var vol)) sb.AppendLine($"- Volume: {vol}");
        if (context.TryGetProperty("ptac", out var ptac)) sb.AppendLine($"- PTAC: {ptac}");
        if (context.TryGetProperty("transmission", out var trans)) sb.AppendLine($"- Transmission: {trans}");
        if (context.TryGetProperty("tvaRecuperable", out var tva)) sb.AppendLine($"- TVA r\u00e9cup\u00e9rable: {tva}");
        if (context.TryGetProperty("title", out var title)) sb.AppendLine($"- Titre: {title}");

        sb.AppendLine("\nR\u00e9ponds UNIQUEMENT avec la description g\u00e9n\u00e9r\u00e9e, sans commentaire ni explication.");
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
        var color = GetValue(context, "color") ?? "";
        var technicalControl = GetValue(context, "technicalControl") ?? "";
        var upholstery = GetValue(context, "upholstery") ?? "";
        var equipment = GetValue(context, "equipment") ?? "";
        var history = GetValue(context, "history") ?? "";

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
        if (!string.IsNullOrEmpty(color)) sb.AppendLine($"- Couleur : {color}");
        if (!string.IsNullOrEmpty(vehicleType)) sb.AppendLine($"- Type de v\u00e9hicule : {vehicleType}");
        if (!string.IsNullOrEmpty(seats)) sb.AppendLine($"- Nombre de si\u00e8ges : {seats}");
        if (!string.IsNullOrEmpty(doors)) sb.AppendLine($"- Nombre de portes : {doors}");
        if (!string.IsNullOrEmpty(fiscalPower)) sb.AppendLine($"- Puissance fiscale : {fiscalPower} CV");
        if (!string.IsNullOrEmpty(technicalControl)) sb.AppendLine($"- Contr\u00f4le technique : Valide jusqu'en {technicalControl}");
        if (!string.IsNullOrEmpty(upholstery)) sb.AppendLine($"- Sellerie : {upholstery}");
        if (!string.IsNullOrEmpty(equipment)) sb.AppendLine($"- \u00c9quipements : {equipment}");
        if (!string.IsNullOrEmpty(history)) sb.AppendLine($"- Historique : {history}");

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
