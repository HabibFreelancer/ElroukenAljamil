using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.Services;

public class AiDescriptionService : IAiDescriptionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiDescriptionService> _logger;
    private readonly IHuggingFaceService _huggingFaceService;

    public AiDescriptionService(IHttpClientFactory httpClientFactory, ILogger<AiDescriptionService> logger, IHuggingFaceService huggingFaceService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _huggingFaceService = huggingFaceService;
    }

    public async Task<string> GenerateDescriptionAsync(JsonElement context, CancellationToken ct = default)
    {
        var prompt = BuildPrompt(context);

        try
        {
            // Appel � l'infrastructure avec propagation du token d'annulation
            var description = await _huggingFaceService.CallHuggingFaceAsync(prompt, ct);
            if (!string.IsNullOrWhiteSpace(description))
            {
                return description;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("La g�n�ration de la description par l'IA a �t� annul�e.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors de l'appel � Hugging Face. Bascule sur le m�canisme de fallback.");
        }

        // Fallback: g�n�ration locale bas�e sur les templates
        return GenerateFallback(context);
    }

    // ==========================================
    // SECTION : LOGIQUE DE FALLBACK
    // ==========================================

    private string GenerateFallback(JsonElement context)
    {
        var category = context.TryGetProperty("category", out var cat) ? cat.GetString() ?? "" : "";
        var propertyType = context.TryGetProperty("propertyType", out var pt) ? pt.GetString() ?? "" : "";
        var isImmobilier = category.Contains("immobilier", StringComparison.OrdinalIgnoreCase) || category.Contains("immobili", StringComparison.OrdinalIgnoreCase);
        var isBureauCommerce = category.Contains("bureau", StringComparison.OrdinalIgnoreCase) || category.Contains("commerce", StringComparison.OrdinalIgnoreCase);

        return isBureauCommerce
            ? GenerateFallbackBureauCommerce(context)
            : isImmobilier
                ? GenerateFallbackImmobilier(context, propertyType)
                : GenerateFallbackDescription(context);
    }

    private string GenerateFallbackDescription(JsonElement context)
    {
        var sb = new StringBuilder();
        var surface = GetValue(context, "surface") ?? "";
        var address = GetValue(context, "address") ?? "";

        sb.Append(!string.IsNullOrEmpty(surface) ? $"Bien de {surface} m� � vendre" : "Bien � vendre");
        if (!string.IsNullOrEmpty(address)) sb.Append($" � {address}");
        sb.AppendLine(".");
        sb.AppendLine("N'h�sitez pas � me contacter pour plus d'informations !");

        return sb.ToString().Trim();
    }

    private string GenerateFallbackImmobilier(JsonElement context, string propertyType)
    {
        var sb = new StringBuilder();
        var surface = GetValue(context, "surface") ?? "";
        var rooms = GetValue(context, "rooms") ?? "";
        var bedrooms = GetValue(context, "bedrooms") ?? "";
        var address = GetValue(context, "address") ?? "";

        sb.Append(!string.IsNullOrEmpty(surface) ? $"Belle propri�t� de {surface} m�" : "Beau bien immobilier");
        if (!string.IsNullOrEmpty(address)) sb.Append($" situ�e � {address}");
        sb.AppendLine(".");
        if (!string.IsNullOrEmpty(rooms)) sb.AppendLine($"Il comprend {rooms} pi�ces" + (!string.IsNullOrEmpty(bedrooms) ? $" dont {bedrooms} chambre(s)" : "") + ".");
        sb.AppendLine("N'h�sitez pas � me contacter pour organiser une visite.");

        return sb.ToString().Trim();
    }

    private string GenerateFallbackBureauCommerce(JsonElement context)
    {
        var sb = new StringBuilder();
        var surface = GetValue(context, "surface") ?? "";
        var address = GetValue(context, "address") ?? "";

        sb.Append(!string.IsNullOrEmpty(surface) ? $"Local professionnel de {surface} m�" : "Local professionnel");
        if (!string.IsNullOrEmpty(address)) sb.Append($" � {address}");
        sb.AppendLine(".");
        sb.AppendLine("Id�al pour investisseur ou activit� professionnelle. Contactez-nous pour plus de d�tails.");

        return sb.ToString().Trim();
    }

    // ==========================================
    // SECTION : CONSTRUCTION DES PROMPTS
    // ==========================================

    private string BuildPrompt(JsonElement context)
    {
        var category = GetValue(context, "category") ?? "";
        var propertyType = GetValue(context, "propertyType") ?? "";

        var isImmobilier = category.Contains("immobilier", StringComparison.OrdinalIgnoreCase) || category.Contains("immobili", StringComparison.OrdinalIgnoreCase);
        var isBureauCommerce = category.Contains("bureau", StringComparison.OrdinalIgnoreCase) || category.Contains("commerce", StringComparison.OrdinalIgnoreCase);

        if (isBureauCommerce) return BuildBureauCommercePrompt(context);
        if (isImmobilier) return BuildImmobilierPrompt(context, propertyType);

        var sb = new StringBuilder();
        sb.AppendLine("G�n�re une description d'annonce de vente en fran�ais en te basant sur ces informations :");

        foreach (var prop in context.EnumerateObject())
        {
            sb.AppendLine($"- {prop.Name}: {prop.Value}");
        }

        sb.AppendLine("\nR�ponds UNIQUEMENT avec la description g�n�r�e, sans commentaire.");
        return sb.ToString();
    }

    private string BuildImmobilierPrompt(JsonElement context, string propertyType)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"G�n�re une description d'annonce immobili�re pour un(e) {propertyType} en fran�ais, style professionnel et attractif.");
        sb.AppendLine("Voici les informations du bien :");

        foreach (var prop in context.EnumerateObject())
        {
            sb.AppendLine($"- {prop.Name}: {prop.Value}");
        }

        sb.AppendLine("\nR�ponds UNIQUEMENT avec la description g�n�r�e.");
        return sb.ToString();
    }

    private string BuildBureauCommercePrompt(JsonElement context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("G�n�re une description d'annonce pour un local de type bureau ou commerce en fran�ais.");

        foreach (var prop in context.EnumerateObject())
        {
            sb.AppendLine($"- {prop.Name}: {prop.Value}");
        }

        sb.AppendLine("\nR�ponds UNIQUEMENT avec la description g�n�r�e.");
        return sb.ToString();
    }

    private string? GetValue(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop))
        {
            return prop.ValueKind switch
            {
                JsonValueKind.String => prop.GetString(),
                JsonValueKind.Number => prop.GetDecimal().ToString(),
                JsonValueKind.True => "Oui",
                JsonValueKind.False => "Non",
                _ => prop.ToString()
            };
        }
        return null;
    }
}
