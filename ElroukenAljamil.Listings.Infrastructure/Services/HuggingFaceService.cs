using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HuggingFaceService> _logger;

        public HuggingFaceService(HttpClient httpClient, IConfiguration configuration, ILogger<HuggingFaceService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> CallHuggingFaceAsync(string prompt, CancellationToken ct)
        {
            var hfToken = _configuration["HuggingFace:ApiToken"] ?? "";
            if (string.IsNullOrEmpty(hfToken))
            {
                _logger.LogWarning("HuggingFace:ApiToken non configuré — appel ignoré");
                return null;
            }

            var requestBody = new
            {
                inputs = prompt,
                parameters = new { max_new_tokens = 300, temperature = 0.7 }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hfToken);

            var modelUrl = _configuration["HuggingFace:ModelUrl"] ?? "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.2";
            _logger.LogDebug("Appel HuggingFace — modèle {ModelUrl}", modelUrl);

            var response = await _httpClient.PostAsync(modelUrl, content, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("HuggingFace API — réponse HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var text = doc.RootElement[0].GetProperty("generated_text").GetString();
                _logger.LogDebug("HuggingFace — description générée ({Length} chars)", text?.Length ?? 0);
                return text;
            }

            _logger.LogWarning("HuggingFace — réponse inattendue, aucun texte généré");
            return null;
        }
    }
}
