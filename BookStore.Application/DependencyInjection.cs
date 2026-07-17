using BookStore.Application.Common.Behaviors;
using BookStore.Application.Common.Behavious;
using FluentValidation; // Thêm thư viện này
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BookStore.Application
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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            return services;
        }
    }
}
