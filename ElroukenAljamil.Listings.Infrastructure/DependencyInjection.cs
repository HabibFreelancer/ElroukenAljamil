using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using ElroukenAljamil.Listings.Infrastructure.Repositories;
using ElroukenAljamil.Listings.Infrastructure.EventPublishing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElroukenAljamil.Listings.Application.Interfaces;
using ElroukenAljamil.Listings.Domain.Interfaces;



namespace ElroukenAljamil.Listings.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddListingsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // SQL Server via EF Core
            services.AddDbContext<ListingsDbContext>(options =>
          options.UseSqlServer(
              configuration.GetConnectionString("ListingsDb"),
              b => b.MigrationsAssembly(typeof(ListingsDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IAdTypeRepository, AdTypeRepository>();
            services.AddScoped<IDepositWorkflowRepository, DepositWorkflowRepository>();
            services.AddScoped<IWorkflowStepRepository, WorkflowStepRepository>();
            services.AddScoped<IStepFieldRepository, StepFieldRepository>();

            // Event publishing
            services.AddScoped<ListingIntegrationEventPublisher>();

            // MassTransit + RabbitMQ via BuildingBlocks
            services.AddEventBus(configuration);

            return services;
        }
    }


}
