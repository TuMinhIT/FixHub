using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Interfaces.Rag;
using FixHub.Application.Features.Rag.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Features.Rag.Services;

public sealed class RagSearchService : IRagSearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRagEmbeddingService _embeddingService;

    public RagSearchService(IUnitOfWork unitOfWork, IRagEmbeddingService embeddingService)
    {
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<RagSourceResponse>> SearchAsync(
        string question,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
            throw new BadRequestException("Question is required.");

        if (question.Length > 2000)
            throw new BadRequestException("Question must not exceed 2000 characters.");

        var values = await _embeddingService.GenerateEmbeddingAsync(question.Trim(), cancellationToken);
        if (values.Length != 768)
            throw new InvalidOperationException($"Embedding dimension mismatch. Expected 768, received {values.Length}.");

        var matches = await _unitOfWork.KnowledgeChunkRepository.SearchSimilarAsync(
            new Pgvector.Vector(values),
            Math.Clamp(topK, 1, 20),
            cancellationToken);

        return matches
            .Where(x => x.SimilarityScore >= 0.35)
            .Select(x => new RagSourceResponse
            {
                ArticleId = x.Chunk.ArticleId,
                Title = x.Chunk.Article?.Title ?? string.Empty,
                Content = x.Chunk.Content,
                ChunkIndex = x.Chunk.ChunkIndex,
                Score = Math.Round(x.SimilarityScore, 4)
            })
            .ToList();
    }
}
