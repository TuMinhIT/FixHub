using FixHub.API.Middlewares;
using FixHub.Application;
using FixHub.Application.Common.Models;
using FixHub.Infrastructure;
using FixHub.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.RateLimiting;

namespace FixHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            var dataProtectionPath = builder.Configuration["DataProtection:KeysPath"]
                ?? Path.Combine(AppContext.BaseDirectory, "data-protection-keys");
            builder.Services.AddDataProtection()
                .SetApplicationName("FixHub")
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));

            // Add application services
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e => new ValidationError
                        {
                            Field = x.Key,
                            Message = e.ErrorMessage
                        }))
                        .ToList();

                    var response = new ErrorResponse
                    {
                        success = false,
                        errorMessage = "Data input is not valid!",
                        errors = errors,
                        data = null
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("rag", context => RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins("https://localhost:5173", "http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");
            app.UseMiddleware<ExceptionMiddleware>();
            if (!app.Environment.IsDevelopment() || app.Urls.Any(x => x.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            app.MapControllers();
            app.MapGet("/", () => "Hello World!");
            app.MapGet("/health/live", () => Results.Ok(new { status = "ok" }));

            app.Run();
        }
    }
}

