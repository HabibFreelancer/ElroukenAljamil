using System.Text;
using ElroukenAljamil.BuildingBlocks.Security.Configuration;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ElroukenAljamil.BuildingBlocks.Security.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Configure l'authentification JWT Bearer pour un microservice.
        /// Tous les services partagent la même clé de validation.
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>() ?? new JwtSettings();

            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SectionName));

            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // true en production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                // Support SignalR : le token est passé en query string pour WebSocket
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // Si la requête est pour un Hub SignalR, lire le token depuis le query string
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// Ajoute les policies d'autorisation communes.
        /// </summary>
        public static IServiceCollection AddMarketplaceAuthorization(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RequireSeller", policy =>
                    policy.RequireRole("Seller", "Admin"));

                options.AddPolicy("RequireBuyer", policy =>
                    policy.RequireRole("Buyer", "Seller", "Admin"));

                options.AddPolicy("RequireEmailVerified", policy =>
                    policy.RequireClaim("email_verified", "true"));
            });

            return services;
        }


        /// <summary>
        /// Enregistre ICurrentUserService + IHttpContextAccessor.
        /// À appeler après AddJwtAuthentication.
        /// </summary>
        public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            // HttpContextAccessor nécessaire pour accéder au ClaimsPrincipal
            services.AddHttpContextAccessor();

            // CurrentUserService en Scoped : une instance par requête HTTP
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
