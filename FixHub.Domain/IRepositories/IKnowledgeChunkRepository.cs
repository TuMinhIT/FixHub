using FixHub.Domain.Entities;
using Pgvector;

namespace FixHub.Domain.IRepositories;

public interface IKnowledgeChunkRepository : IRepository<KnowledgeChunk>
{
    Task<IReadOnlyList<KnowledgeChunkMatch>> SearchSimilarAsync(
        Vector queryVector,
        int limit = 5,
        CancellationToken cancellationToken = default);

    Task DeleteByArticleIdAsync(Guid articleId, CancellationToken cancellationToken = default);
}
