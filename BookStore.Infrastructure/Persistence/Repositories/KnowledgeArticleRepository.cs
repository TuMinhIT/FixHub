using FixHub.Domain.Entities;
using FixHub.Domain.IRepositories;


namespace FixHub.Infrastructure.Persistence.Repositories
{
    public class KnowledgeArticleRepository : Repository<KnowledgeArticle>, IKnowledgeArticleRepository
    {
        public KnowledgeArticleRepository(AppDbContext context) : base(context)
        {
        }

    }
}
