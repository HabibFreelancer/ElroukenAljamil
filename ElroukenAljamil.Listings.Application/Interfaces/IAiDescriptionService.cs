using System.Text.Json;

namespace ElroukenAljamil.Listings.Application.Interfaces;

public interface IAiDescriptionService
{
    Task<string> GenerateAsync(JsonElement context, CancellationToken ct = default);
}
