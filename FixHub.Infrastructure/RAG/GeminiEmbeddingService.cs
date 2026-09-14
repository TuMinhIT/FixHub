using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces.Rag;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;

namespace FixHub.Infrastructure.RAG;

public sealed class GeminiEmbeddingService : IRagEmbeddingService
{
    private const int EmbeddingDimension = 768;

    private readonly Client _client;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiEmbeddingService(Client client, IConfiguration configuration)
    {
        _client = client;
        _apiKey = configuration["Gemini:ApiKey"]?.Trim() ?? string.Empty;
        _model = configuration["Gemini:EmbeddingModel"];
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new ServiceUnavailableException("Gemini embedding provider is not configured.");

        try
        {
            var response = await _client.Models.EmbedContentAsync(
                model: _model,
                contents: text.Trim(),
                config: new EmbedContentConfig
                {
                    // The pgvector column and RAG services are configured for vector(768).
                    OutputDimensionality = EmbeddingDimension
                },
                cancellationToken: cancellationToken);

            var values = response.Embeddings?.FirstOrDefault()?.Values;
            if (values is null || values.Count == 0)
                throw new ServiceUnavailableException("Gemini embedding provider returned no values.");

            var vector = values
                .Select(Convert.ToSingle)
                .ToArray();
            if (vector.Length != EmbeddingDimension)
            {
                throw new ServiceUnavailableException(
                    $"Gemini embedding model returned {vector.Length} dimensions; " +
                    $"the application requires {EmbeddingDimension}.");
            }

            return Normalize(vector);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ServiceUnavailableException("Gemini embedding provider timed out.");
        }
        catch (ClientError exception)
        {
            throw new ServiceUnavailableException(
                $"Gemini embedding request failed for model '{_model}'. {exception.Message}",
                exception);
        }
        catch (ServerError exception)
        {
            throw new ServiceUnavailableException(
                $"Gemini embedding provider returned a server error for model '{_model}'.",
                exception);
        }
        catch (HttpRequestException exception)
        {
            throw new ServiceUnavailableException(
                "Gemini embedding provider is unavailable.",
                exception);
        }
    }

    private static float[] Normalize(float[] values)
    {
        var norm = Math.Sqrt(values.Sum(value => (double)value * value));
        if (norm <= double.Epsilon)
            throw new ServiceUnavailableException("Gemini returned a zero-length embedding vector.");

        return values
            .Select(value => (float)(value / norm))
            .ToArray();
    }
}
