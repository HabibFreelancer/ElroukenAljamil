using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ElroukenAljamil.Notification.Infrastructure.Services
{
    public class HttpRecipientResolver : IRecipientResolver
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public HttpRecipientResolver(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<RecipientInfo?> ResolveAsync(Guid userId, CancellationToken ct = default)
        {
            var baseUrl = _config["Services:IdentityApi"] ?? "http://identity-api";
            var response = await _http.GetAsync($"{baseUrl}/api/users/{userId}/info", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json).RootElement;

            return new RecipientInfo(
                userId,
                doc.GetProperty("fullName").GetString() ?? "",
                doc.GetProperty("email").GetString() ?? "",
                doc.TryGetProperty("phoneNumber", out var phone) ? phone.GetString() : null
            );
        }
    }
}
