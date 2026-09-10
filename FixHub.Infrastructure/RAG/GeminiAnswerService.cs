using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.Rag.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FixHub.Application.Common.Interfaces.Rag;

namespace FixHub.Infrastructure.RAG;

public sealed class GeminiAnswerService : IRagAnswerService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAnswerService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        _model = configuration["Gemini:AnswerModel"] ?? "gemini-2.0-flash";
    }

    public async Task<string> GenerateAnswerAsync(
        string question,
        IReadOnlyList<RagSourceResponse> sources,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new ServiceUnavailableException("Gemini answer provider is not configured.");

        var context = string.Join(
            "\n\n",
            sources.Select((source, index) =>
                $"Nguồn {index + 1}: {source.Title}\n{source.Content}"));

        var prompt = $"""
            Bạn là trợ lý kỹ thuật của FixHub về thiết bị điện lạnh dân dụng.
            Chỉ sử dụng thông tin trong CONTEXT để trả lời QUESTION.
            Nếu CONTEXT không đủ, nói rõ rằng chưa đủ dữ liệu và khuyến nghị người dùng liên hệ kỹ thuật viên.
            Không khẳng định chẩn đoán tuyệt đối. Với nguy cơ điện, gas lạnh hoặc cháy nổ, luôn khuyến nghị ngắt điện và gọi kỹ thuật viên.
            Trả lời bằng tiếng Việt, ngắn gọn, theo các bước dễ làm và không bịa thêm thông số.

            QUESTION:
            {question}

            CONTEXT:
            {context}
            """;

        var request = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[] { new { text = prompt } }
                }
            },
            generationConfig = new
            {
                temperature = 0.2,
                maxOutputTokens = 800
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException exception)
        {
            throw new ServiceUnavailableException("Gemini answer provider is unavailable.", exception);
        }

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(cancellationToken));
        var root = document.RootElement;
        var text = root.TryGetProperty("candidates", out var candidates)
            && candidates.GetArrayLength() > 0
            && candidates[0].TryGetProperty("content", out var content)
            && content.TryGetProperty("parts", out var parts)
            && parts.GetArrayLength() > 0
            && parts[0].TryGetProperty("text", out var textElement)
                ? textElement.GetString()
                : null;

        return string.IsNullOrWhiteSpace(text)
            ? "Chưa tạo được câu trả lời. Vui lòng thử lại sau."
            : text.Trim();
    }
}
