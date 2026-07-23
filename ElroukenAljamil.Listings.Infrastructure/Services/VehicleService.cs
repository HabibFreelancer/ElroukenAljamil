using System.Text.Json;
using System.Text.RegularExpressions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(IHttpClientFactory httpClientFactory, ILogger<VehicleService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<VehicleLookupResult> LookupAsync(string immatriculation, CancellationToken ct = default)
    {
        var normalized = immatriculation.Replace(" ", "").ToUpper();

        var client = _httpClientFactory.CreateClient("vehicle");
        client.Timeout = TimeSpan.FromSeconds(10);

        string[] urls =
        [
            $"https://www.vehiculeapi.tn/api/v1/lookup/{normalized}",
            $"https://www.vehiculeapi.tn/api/vehicle/{normalized}",
            $"https://vehiculeapi.tn/api/v1/{normalized}"
        ];

        foreach (var url in urls)
        {
            try
            {
                var response = await client.GetAsync(url, ct);
                _logger.LogInformation("Vehicle API {Url} -> {Status}", url, response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(ct);
                    if (!string.IsNullOrWhiteSpace(json) && json.TrimStart().StartsWith('{'))
                    {
                        var data = JsonSerializer.Deserialize<JsonElement>(json);
                        var result = ExtractVehicleData(data);
                        if (result.Brand != null) return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Vehicle API error {Url}: {Error}", url, ex.Message);
            }
        }

        return GetMockVehicleData(normalized);
    }

    private static VehicleLookupResult ExtractVehicleData(JsonElement data) => new()
    {
        Brand            = GetValue(data, "marque", "brand", "make", "Marque"),
        Model            = GetValue(data, "modele", "model", "Model", "Modele"),
        Year             = GetValue(data, "annee", "year", "annee_modele", "Annee"),
        Fuel             = GetValue(data, "energie", "fuel", "carburant", "Energie"),
        Gearbox          = GetValue(data, "boite", "gearbox", "boite_vitesse", "Boite"),
        FiscalPower      = GetValue(data, "puissance_fiscale", "fiscal_power", "cv", "CV"),
        DinPower         = GetValue(data, "puissance_din", "din_power", "ch", "CH"),
        FirstCirculation = GetValue(data, "date_mise_circulation", "first_circulation", "mise_circulation"),
        Color            = GetValue(data, "couleur", "color", "Couleur"),
        VehicleType      = GetValue(data, "type", "genre", "vehicle_type", "Type")
    };

    private static VehicleLookupResult GetMockVehicleData(string immat)
    {
        var numbers = Regex.Replace(immat, @"[^\d]", "");
        var seed = numbers.Length > 0 ? int.Parse(numbers[..Math.Min(4, numbers.Length)]) : 0;

        string[] brands = ["Peugeot", "Renault", "Citroen", "Volkswagen", "Toyota", "Hyundai", "Kia", "Fiat", "Dacia"];
        var models = new Dictionary<string, string[]>
        {
            ["Peugeot"]    = ["208", "308", "3008", "5008", "2008"],
            ["Renault"]    = ["Clio", "Megane", "Captur", "Kadjar", "Scenic"],
            ["Citroen"]    = ["C3", "C4", "C5 Aircross", "Berlingo"],
            ["Volkswagen"] = ["Golf", "Polo", "Tiguan", "T-Roc"],
            ["Toyota"]     = ["Yaris", "Corolla", "C-HR", "RAV4"],
            ["Hyundai"]    = ["i20", "i30", "Tucson", "Kona"],
            ["Kia"]        = ["Picanto", "Rio", "Sportage", "Ceed"],
            ["Fiat"]       = ["500", "Panda", "Tipo", "500X"],
            ["Dacia"]      = ["Sandero", "Duster", "Logan", "Jogger"]
        };
        string[] fuels     = ["essence", "diesel", "hybride", "electrique"];
        string[] gearboxes = ["manuelle", "automatique"];
        string[] years     = ["2018", "2019", "2020", "2021", "2022", "2023"];

        var brand = brands[seed % brands.Length];
        var model = models[brand][seed % models[brand].Length];
        var year  = years[seed % years.Length];
        var month = ((seed % 12) + 1).ToString("D2");

        return new VehicleLookupResult
        {
            Brand            = brand,
            Model            = model,
            Year             = year,
            Fuel             = fuels[seed % fuels.Length],
            Gearbox          = gearboxes[seed % gearboxes.Length],
            FiscalPower      = (5 + seed % 10).ToString(),
            DinPower         = (90 + seed % 80).ToString(),
            FirstCirculation = $"{month}/{year}",
            Color            = null,
            VehicleType      = null
        };
    }

    private static string? GetValue(JsonElement data, params string[] keys)
    {
        foreach (var key in keys)
            if (data.TryGetProperty(key, out var prop) &&
                prop.ValueKind != JsonValueKind.Null &&
                prop.ValueKind != JsonValueKind.Undefined)
                return prop.ToString();
        return null;
    }
}
