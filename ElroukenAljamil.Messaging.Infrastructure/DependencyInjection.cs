using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using ElroukenAljamil.Messaging.Infrastructure.Persistence;
using ElroukenAljamil.Messaging.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElroukenAljamil.Messaging.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- PostgreSQL + EF Core ---
            services.AddDbContext<MessagingDbContext>(options =>
                     options.UseSqlServer(
          configuration.GetConnectionString("MessagingDb"),
          b => b.MigrationsAssembly(typeof(MessagingDbContext).Assembly.FullName))


                );

            // --- Repositories ---
            services.AddScoped<IConversationRepository, ConversationRepository>();

            // --- EventBus (MassTransit + RabbitMQ) via BuildingBlocks ---
            services.AddEventBus(configuration);

            return services;
        }
    }
}
