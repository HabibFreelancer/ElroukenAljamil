using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public VehicleController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("lookup/{immatriculation}")]
    public async Task<ActionResult> Lookup(string immatriculation)
    {
        if (string.IsNullOrWhiteSpace(immatriculation))
            return BadRequest(new { message = "Immatriculation requise." });

        // Normalize: remove spaces
        var normalized = immatriculation.Replace(" ", "").ToUpper();

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            // Call vehiculeapi.tn
            var response = await client.GetAsync($"https://www.vehiculeapi.tn/api/v1/lookup/{normalized}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                return Ok(new
                {
                    brand = GetJsonValue(data, "marque", "brand", "make"),
                    model = GetJsonValue(data, "modele", "model"),
                    year = GetJsonValue(data, "annee", "year", "annee_modele"),
                    fuel = GetJsonValue(data, "energie", "fuel", "carburant"),
                    gearbox = GetJsonValue(data, "boite", "gearbox", "boite_vitesse"),
                    fiscalPower = GetJsonValue(data, "puissance_fiscale", "fiscal_power", "cv"),
                    dinPower = GetJsonValue(data, "puissance_din", "din_power", "ch"),
                    firstCirculation = GetJsonValue(data, "date_mise_circulation", "first_circulation", "mise_circulation"),
                    color = GetJsonValue(data, "couleur", "color"),
                    vehicleType = GetJsonValue(data, "type", "genre", "vehicle_type")
                });
            }

            // If API returns error, return empty but valid response
            return Ok(new { brand = (string?)null });
        }
        catch
        {
            return Ok(new { brand = (string?)null });
        }
    }

    private string? GetJsonValue(JsonElement data, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (data.TryGetProperty(key, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return prop.ToString();
        }
        return null;
    }
}
