using FixHub.Application.Common.Interfaces;
using FixHub.Application.Payment;
using FixHub.Domain.IRepositories;
using FixHub.Infrastructure.Authentication;
using FixHub.Infrastructure.Payment;
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
            services.Configure<SePayOptions>(configuration.GetSection(SePayOptions.SectionName));

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IKnowledgeArticleRepository, KnowledgeArticleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPaymentGateway, SePayGateway>();

            services.AddHttpClient<IRagEmbeddingService, FixHub.Infrastructure.RAG.GeminiEmbeddingService>();

            return services;
        }
    }
}
