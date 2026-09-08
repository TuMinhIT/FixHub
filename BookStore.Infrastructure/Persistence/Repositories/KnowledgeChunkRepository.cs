using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace FixHub.Infrastructure.Persistence.Repositories;

public class KnowledgeChunkRepository : Repository<KnowledgeChunk>, IKnowledgeChunkRepository
{
    public KnowledgeChunkRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<KnowledgeChunkMatch>> SearchSimilarAsync(
        Vector queryVector,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeChunks
            .Where(x => x.Article.Status == KnowledgeArticleStatuses.Published)
            .Include(x => x.Article)
            .OrderBy(x => x.Embedding.CosineDistance(queryVector))
            .Select(x => new KnowledgeChunkMatch
            {
                Chunk = x,
                SimilarityScore = 1 - x.Embedding.CosineDistance(queryVector)
            })
            .Take(Math.Clamp(limit, 1, 20))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeChunks
            .Where(x => x.ArticleId == articleId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
