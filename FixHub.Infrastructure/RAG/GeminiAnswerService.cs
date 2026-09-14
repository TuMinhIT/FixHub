using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.Rag.DTOs;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using FixHub.Application.Common.Interfaces.Rag;

namespace FixHub.Infrastructure.RAG;

public sealed class GeminiAnswerService : IRagAnswerService
{
    private readonly Client _client;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAnswerService(Client client, IConfiguration configuration)
    {
        _client = client;
        _apiKey = configuration["Gemini:ApiKey"]?.Trim() ?? string.Empty;
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

        try
        {
            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: new GenerateContentConfig
                {
                    Temperature = 0.2,
                    MaxOutputTokens = 800
                },
                cancellationToken: cancellationToken);

            var text = response.Text;
            return string.IsNullOrWhiteSpace(text)
                ? "Chưa tạo được câu trả lời. Vui lòng thử lại sau."
                : text.Trim();
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ServiceUnavailableException("Gemini answer provider timed out.");
        }
        catch (ClientError exception)
        {
            throw new ServiceUnavailableException(
                $"Gemini answer request failed for model '{_model}'. {exception.Message}",
                exception);
        }
        catch (ServerError exception)
        {
            throw new ServiceUnavailableException(
                $"Gemini answer provider returned a server error for model '{_model}'.",
                exception);
        }
        catch (HttpRequestException exception)
        {
            throw new ServiceUnavailableException("Gemini answer provider is unavailable.", exception);
        }
    }
}
