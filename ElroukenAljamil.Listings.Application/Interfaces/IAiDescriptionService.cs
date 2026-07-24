using System.Text.Json;

namespace ElroukenAljamil.Listings.Application.Interfaces;

public interface IAiDescriptionService
{
    Task<string> GenerateDescriptionAsync(JsonElement context, CancellationToken ct = default);
}
