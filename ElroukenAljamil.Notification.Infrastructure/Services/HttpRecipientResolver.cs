using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ElroukenAljamil.Notification.Infrastructure.Services
{
    public class HttpRecipientResolver : IRecipientResolver
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<HttpRecipientResolver> _logger;

        public HttpRecipientResolver(HttpClient http, IConfiguration config, ILogger<HttpRecipientResolver> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }

        public async Task<RecipientInfo?> ResolveAsync(Guid userId, CancellationToken ct = default)
        {
            var baseUrl = _config["Services:IdentityApi"] ?? "http://identity-api";
            var response = await _http.GetAsync($"{baseUrl}/api/users/{userId}/info", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ResolveAsync — utilisateur {UserId} introuvable (HTTP {StatusCode})",
                    userId, (int)response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json).RootElement;

            return new RecipientInfo(
                userId,
                doc.GetProperty("fullName").GetString() ?? "",
                doc.GetProperty("email").GetString() ?? "",
                doc.TryGetProperty("phoneNumber", out var phone) ? phone.GetString() : null
            );
        }

        public async Task<IEnumerable<Guid>> GetAdminUserIdsAsync(CancellationToken ct = default)
        {
            try
            {
                var baseUrl = _config["Services:IdentityApi"] ?? "http://identity-api";
                var response = await _http.GetAsync($"{baseUrl}/api/users/admins", ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GetAdminUserIdsAsync — HTTP {StatusCode}", (int)response.StatusCode);
                    return [];
                }

                var json = await response.Content.ReadAsStringAsync(ct);
                var ids = JsonSerializer.Deserialize<List<Guid>>(json) ?? [];
                _logger.LogDebug("GetAdminUserIdsAsync — {Count} admin(s) récupéré(s)", ids.Count);
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAdminUserIdsAsync — échec appel Identity API");
                return [];
            }
        }
    }
}
