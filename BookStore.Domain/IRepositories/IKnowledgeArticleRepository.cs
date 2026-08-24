using FixHub.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pgvector;

namespace FixHub.Domain.IRepositories
{
    public interface IKnowledgeArticleRepository : IRepository<KnowledgeArticle>
    {
        Task<IEnumerable<KnowledgeArticle>> SearchSimilarAsync(Vector queryVector, int limit = 5);
    }
}
