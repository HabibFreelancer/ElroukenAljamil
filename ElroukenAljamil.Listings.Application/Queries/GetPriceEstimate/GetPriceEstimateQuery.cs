using System.Text.Json;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetPriceEstimate;

public record GetPriceEstimateQuery(int CategoryId, string Brand, string Model) : IRequest<PriceEstimateDto>;

public class GetPriceEstimateQueryHandler : IRequestHandler<GetPriceEstimateQuery, PriceEstimateDto>
{
    private readonly IAnnonceRepository _repository;
    public GetPriceEstimateQueryHandler(IAnnonceRepository repository) => _repository = repository;

    public async Task<PriceEstimateDto> Handle(GetPriceEstimateQuery request, CancellationToken ct)
    {
        var allAds = (await _repository.GetByCategoryForEstimateAsync(request.CategoryId, 100)).ToList();

        var similar = allAds
            .Where(a => string.IsNullOrEmpty(request.Brand) ||
                        a.ExtraData.Contains(request.Brand, StringComparison.OrdinalIgnoreCase))
            .Take(10).ToList();

        if (similar.Count < 3) similar = allAds.Take(10).ToList();

        var prices = similar.Where(a => a.Price > 0).Select(a => a.Price).ToList();

        var result = new PriceEstimateDto
        {
            MinPrice = prices.Count > 0 ? prices.Min() : 0,
            MaxPrice = prices.Count > 0 ? prices.Max() : 0,
            AvgPrice = prices.Count > 0 ? prices.Average() : 0,
            Count    = prices.Count,
            SimilarAds = similar.Take(5).Select(a => new SimilarAdDto
            {
                Id       = a.Id,
                Title    = a.Title,
                Price    = a.Price,
                Mileage  = ExtractField(a.ExtraData, "mileage"),
                Location = a.Location,
                Image    = $"https://placehold.co/200x130/f0f0f0/666?text={Uri.EscapeDataString(ExtractBrandModel(a.ExtraData))}"
            }).ToList()
        };

        return result;
    }

    private static string ExtractField(string extraData, string key)
    {
        if (string.IsNullOrEmpty(extraData)) return "";
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(extraData);
            return data.TryGetProperty(key, out var val) ? val.ToString() : "";
        }
        catch { return ""; }
    }

    private static string ExtractBrandModel(string extraData)
    {
        if (string.IsNullOrEmpty(extraData)) return "Auto";
        try
        {
            var data  = JsonSerializer.Deserialize<JsonElement>(extraData);
            var brand = data.TryGetProperty("brand", out var b) ? b.ToString() : "";
            var model = data.TryGetProperty("model", out var m) ? m.ToString() : "";
            var result = $"{brand} {model}".Trim();
            return string.IsNullOrEmpty(result) ? "Auto" : result;
        }
        catch { return "Auto"; }
    }
}
