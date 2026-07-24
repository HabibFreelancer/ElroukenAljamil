using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ElroukenAljamil.Listings.Infrastructure.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HuggingFaceService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string?> CallHuggingFaceAsync(string prompt, CancellationToken ct)
        {
            var hfToken = _configuration["HuggingFace:ApiToken"] ?? "";
            if (string.IsNullOrEmpty(hfToken)) return null;

            var requestBody = new
            {
                inputs = prompt,
                parameters = new { max_new_tokens = 300, temperature = 0.7 }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hfToken);

            var modelUrl = _configuration["HuggingFace:ModelUrl"] ?? "https://api-inference.huggingface.co/models/mistralai/Mistral-7B-Instruct-v0.2";

            // Passage du CancellationToken ici pour stopper l'appel HTTP si besoin
            var response = await _httpClient.PostAsync(modelUrl, content, ct);
            if (!response.IsSuccessStatusCode) return null;

            var responseString = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                return doc.RootElement[0].GetProperty("generated_text").GetString();
            }

            return null;
        }
    }
}
