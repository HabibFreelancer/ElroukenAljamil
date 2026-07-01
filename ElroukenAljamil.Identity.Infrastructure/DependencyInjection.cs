using ElroukenAljamil.BuildingBlocks.EventBus;
using ElroukenAljamil.BuildingBlocks.Security.Configuration;
using ElroukenAljamil.Identity.Domain.Interfaces;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using ElroukenAljamil.Identity.Infrastructure.Repositories;
using ElroukenAljamil.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using IdentityDbContext = ElroukenAljamil.Identity.Infrastructure.Persistence.IdentityDbContext;
using ElroukenAljamil.Identity.Application.Interfaces;


namespace ElroukenAljamil.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // SQLServer
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityDb"),
                    b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            // JWT Token Generator (utilise JwtSettings de BuildingBlocks.Security)
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            // MassTransit + RabbitMQ via BuildingBlocks
            services.AddEventBus(configuration);

            return services;
        }
    }

}
