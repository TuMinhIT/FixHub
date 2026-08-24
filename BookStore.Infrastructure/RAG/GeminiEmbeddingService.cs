using FixHub.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace FixHub.Infrastructure.RAG
{
    public class GeminiEmbeddingService : IRagEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiEmbeddingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Expecting Gemini:ApiKey in appsettings.json
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                // Fallback for demonstration if API key is not set
                return new float[768]; 
            }

            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/embedding-001:embedContent?key={_apiKey}";
            
            var requestBody = new
            {
                model = "models/embedding-001",
                content = new
                {
                    parts = new[]
                    {
                        new { text = text }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(requestUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

            // Extract the embedding values
            var values = result.GetProperty("embedding").GetProperty("values").EnumerateArray().Select(x => x.GetSingle()).ToArray();
            
            return values;
        }
    }
}
