using System.Text.Json;
using System.Text.RegularExpressions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.Services;

public class AiDescriptionService : IAiDescriptionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VehicleService> _logger;

    public AiDescriptionService(IHttpClientFactory httpClientFactory, ILogger<VehicleService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Task<string> GenerateAsync(JsonElement context, CancellationToken ct = default)
    {
        return Task.FromResult("Generated AI Description");
    }
}
