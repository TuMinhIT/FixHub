using FixHub.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FixHub.Application.Common.Interfaces;
using FixHub.Application.Features.Rag.Services;
using FixHub.Application.Features.Orders.Services;
using FixHub.Application.Common.Interfaces.Rag;

namespace FixHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            // Register MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            services.AddScoped<IRagIndexingService, RagIndexingService>();
            services.AddScoped<IRagSearchService, RagSearchService>();
            services.AddScoped<IOrderInventoryService, OrderInventoryService>();

            return services;
        }
    }
}
