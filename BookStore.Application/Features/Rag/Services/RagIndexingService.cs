using System.Security.Cryptography;
using System.Text;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Common.Interfaces;
using FixHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FixHub.Application.Features.Rag.Services;

public sealed class RagIndexingService : IRagIndexingService
{
    private const int EmbeddingDimension = 768;
    private const int MaxChunkCharacters = 2800;
    private const int OverlapCharacters = 300;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRagEmbeddingService _embeddingService;

    public RagIndexingService(IUnitOfWork unitOfWork, IRagEmbeddingService embeddingService)
    {
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
    }

    public async Task IndexArticleAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.KnowledgeArticleRepository.FindById(articleId);

        if (article == null)
            throw new NotFoundException(nameof(KnowledgeArticle), articleId);

        await _unitOfWork.KnowledgeChunkRepository.DeleteByArticleIdAsync(articleId, cancellationToken);

        var source = string.Join(
            "\n\n",
            new[] { article.Title, article.Tags, article.Content }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        var chunks = Chunk(source)
            .Select((content, index) => new { content, index })
            .ToList();

        foreach (var chunk in chunks)
        {
            var values = await _embeddingService.GenerateEmbeddingAsync(chunk.content, cancellationToken);
            if (values.Length != EmbeddingDimension)
                throw new InvalidOperationException($"Embedding dimension mismatch. Expected {EmbeddingDimension}, received {values.Length}.");

            await _unitOfWork.KnowledgeChunkRepository.AddAsync(new KnowledgeChunk
            {
                ArticleId = article.Id,
                ChunkIndex = chunk.index,
                Content = chunk.content,
                Embedding = new Pgvector.Vector(values),
                TokenCount = chunk.content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                ContentHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(chunk.content))).ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReindexAllAsync(CancellationToken cancellationToken = default)
    {
        var ids = await _unitOfWork.KnowledgeArticleRepository
            .GetAll()
            .Where(x => x.Status == KnowledgeArticleStatuses.Published)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in ids)
            await IndexArticleAsync(id, cancellationToken);
    }

    private static IReadOnlyList<string> Chunk(string text)
    {
        var normalized = string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        if (normalized.Length == 0)
            return Array.Empty<string>();

        var result = new List<string>();
        var start = 0;
        while (start < normalized.Length)
        {
            var end = Math.Min(start + MaxChunkCharacters, normalized.Length);
            if (end < normalized.Length)
            {
                var whitespace = normalized.LastIndexOf(' ', end - 1, end - start);
                if (whitespace > start)
                    end = whitespace;
            }

            result.Add(normalized[start..end].Trim());
            if (end == normalized.Length)
                break;

            start = Math.Max(end - OverlapCharacters, start + 1);
        }

        return result;
    }
}
