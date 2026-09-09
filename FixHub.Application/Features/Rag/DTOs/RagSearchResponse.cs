namespace FixHub.Application.Features.Rag.DTOs;

public sealed class RagSearchResponse
{
    public IReadOnlyList<RagSourceResponse> Sources { get; init; } = Array.Empty<RagSourceResponse>();
}
