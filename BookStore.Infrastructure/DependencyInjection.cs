using FixHub.Application.Common.Interfaces;
using FixHub.Domain.IRepositories;
using FixHub.Infrastructure.Authentication;
using FixHub.Infrastructure.Persistence;
using FixHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FixHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(
                options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")
                    //npgsql =>
                    //    npgsql.UseVector()

                    )

                );
            // Register other infrastructure services here

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IKnowledgeArticleRepository, KnowledgeArticleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtService, JwtService>();
            
            services.AddHttpClient<IRagEmbeddingService, FixHub.Infrastructure.RAG.GeminiEmbeddingService>();

            return services;
        }
    }
}
