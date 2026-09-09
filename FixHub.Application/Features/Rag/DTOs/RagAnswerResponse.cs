namespace FixHub.Application.Features.Rag.DTOs;

public sealed class RagAnswerResponse
{
    public string Answer { get; init; } = string.Empty;
    public IReadOnlyList<RagSourceResponse> Sources { get; init; } = Array.Empty<RagSourceResponse>();
    public IReadOnlyList<object> SuggestedProducts { get; init; } = Array.Empty<object>();
    public IReadOnlyList<object> SuggestedServices { get; init; } = Array.Empty<object>();
    public double Confidence { get; init; }
}
