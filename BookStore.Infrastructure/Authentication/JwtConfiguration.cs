using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FixHub.Infrastructure.Authentication
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind JwtSettings từ appsettings.json
            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SectionName));

            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT configuration is missing.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
                throw new InvalidOperationException("JWT Secret is missing.");

            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration
                        .GetSection(JwtSettings.SectionName)
                        .Get<JwtSettings>()
                        ?? throw new InvalidOperationException("JWT configuration is missing.");

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.MapInboundClaims = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization();
            return services;
        }
    }


}


