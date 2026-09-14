using System.Net;
using System.Text.Json;
using FixHub.Infrastructure.RAG;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FixHub.Tests;

public sealed class GeminiEmbeddingServiceTests
{
    [Fact]
    public async Task GenerateEmbeddingAsync_UsesGeminiModelHeaderAnd768Dimensions()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new StubHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    embeddings = new[]
                    {
                        new { values = Enumerable.Repeat(1.0f, 768).ToArray() }
                    }
                }))
            });
        });

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gemini:ApiKey"] = "test-key",
                ["Gemini:EmbeddingModel"] = "models/embedding-001"
            })
            .Build();

        using var client = new Client(
            apiKey: "test-key",
            clientOptions: new ClientOptions
            {
                HttpClientFactory = () => new HttpClient(handler)
            });
        var service = new GeminiEmbeddingService(client, configuration);
        var values = await service.GenerateEmbeddingAsync("  test text  ");

        Assert.NotNull(capturedRequest);
        var requestBody = await capturedRequest!.Content!.ReadAsStringAsync();
        Assert.Contains("outputDimensionality", requestBody);
        Assert.Equal(768, values.Length);
        Assert.Equal(1d, Math.Sqrt(values.Sum(value => (double)value * value)), precision: 5);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_ReportsProviderErrorWithoutExposingApiKey()
    {
        var handler = new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"error\":{\"message\":\"Model not found\"}}")
        }));

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Gemini:ApiKey"] = "secret-test-key",
                ["Gemini:EmbeddingModel"] = "gemini-embedding-001"
            })
            .Build();

        using var client = new Client(
            apiKey: "secret-test-key",
            clientOptions: new ClientOptions
            {
                HttpClientFactory = () => new HttpClient(handler)
            });
        var service = new GeminiEmbeddingService(client, configuration);
        var exception = await Assert.ThrowsAsync<FixHub.Application.Common.Exceptions.ServiceUnavailableException>(
            () => service.GenerateEmbeddingAsync("test"));

        Assert.Contains("Gemini embedding request failed", exception.Message);
        Assert.DoesNotContain("secret-test-key", exception.Message);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => _handler(request);
    }
}
