namespace FixHub.Application.Features.Rag.DTOs;

public sealed class RagSourceResponse
{
    public Guid ArticleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public int ChunkIndex { get; init; }
    public double Score { get; init; }
}
