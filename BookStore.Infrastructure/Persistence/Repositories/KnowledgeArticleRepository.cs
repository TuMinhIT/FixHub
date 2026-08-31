using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Pgvector;


namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class KnowledgeArticleRepository : Repository<KnowledgeArticle>, IKnowledgeArticleRepository
    {
        public KnowledgeArticleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<KnowledgeArticle>> SearchSimilarAsync(Vector queryVector, int limit = 5)
        {
            // Use CosineDistance to order by similarity
            return await _context.KnowledgeArticles
                //.OrderBy(a => a.Embedding!.CosineDistance(queryVector))
                //.Take(limit)
                .ToListAsync();
        }
    }
}
