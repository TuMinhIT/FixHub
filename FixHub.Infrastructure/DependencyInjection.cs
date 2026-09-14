using FixHub.Application.Common.Interfaces;
using FixHub.Application.Payment;
using FixHub.Domain.IRepositories;
using FixHub.Domain.Entities;
using FixHub.Infrastructure.Authentication;
using FixHub.Infrastructure.Payment;
using FixHub.Infrastructure.Persistence;
using FixHub.Infrastructure.Persistence.Repositories;
using FixHub.Infrastructure.Storage;
using FixHub.Infrastructure.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Google.GenAI;
using FixHub.Application.Common.Interfaces.Rag;
using FixHub.Infrastructure.RAG;
namespace FixHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SePayOptions>(configuration.GetSection(SePayOptions.SectionName));

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsql => npgsql.UseVector()));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IKnowledgeArticleRepository, KnowledgeArticleRepository>();
            services.AddScoped<IKnowledgeChunkRepository, KnowledgeChunkRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IRepository<InventoryTransaction>, Repository<InventoryTransaction>>();
            services.AddScoped<IRepository<StockReservation>, Repository<StockReservation>>();
            services.AddScoped<IRepository<PaymentEvent>, Repository<PaymentEvent>>();
            services.AddScoped<IRepository<RagFeedback>, Repository<RagFeedback>>();
            services.AddScoped<IRepository<UploadedImage>, Repository<UploadedImage>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPaymentGateway, SePayGateway>();
            services.AddScoped<IImageStorageService, CloudinaryService>();

            services.AddSingleton(_ => new Client(
                apiKey: configuration["Gemini:ApiKey"]?.Trim() ?? string.Empty));
            services.AddScoped<IRagEmbeddingService, GeminiEmbeddingService>();
            services.AddScoped<IRagAnswerService, GeminiAnswerService>();
            services.AddSingleton<IRagIndexQueue, RagIndexQueue>();
            services.AddHostedService<RagIndexWorker>();
            services.AddHostedService<StockReservationCleanupService>();

            return services;
        }
    }
}
