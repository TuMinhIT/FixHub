namespace FixHub.Domain.Entities;

public sealed class KnowledgeChunkMatch
{
    public KnowledgeChunk Chunk { get; init; } = null!;
    public double SimilarityScore { get; init; }
}
