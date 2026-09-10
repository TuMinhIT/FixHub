using FixHub.Application.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FixHub.Application.Common.Interfaces.Rag;

namespace FixHub.Infrastructure.RAG
{
    public class GeminiEmbeddingService : IRagEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiEmbeddingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Expecting Gemini:ApiKey in appsettings.json
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            _model = configuration["Gemini:EmbeddingModel"] ?? "embedding-001";
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ServiceUnavailableException("Gemini embedding provider is not configured.");
            }

            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:embedContent?key={_apiKey}";
            
            var requestBody = new
            {
                model = $"models/{_model}",
                content = new
                {
                    parts = new[]
                    {
                        new { text = text }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(requestUrl, jsonContent, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException exception)
            {
                throw new ServiceUnavailableException("Gemini embedding provider is unavailable.", exception);
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

            // Extract the embedding values
            var values = result.GetProperty("embedding").GetProperty("values").EnumerateArray().Select(x => x.GetSingle()).ToArray();
            
            return values;
        }
    }
}
