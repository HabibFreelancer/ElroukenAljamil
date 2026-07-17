using ElroukenAljamil.BuildingBlocks.EventBus;
using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using ElroukenAljamil.Search.Domain.Interfaces;
using ElroukenAljamil.Search.Infrastructure.Consumers;
using ElroukenAljamil.Search.Infrastructure.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElroukenAljamil.Search.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- Elasticsearch ---
            services.AddSingleton<IElasticClient>(sp =>
            {
                var elasticConfig = configuration.GetSection("Elasticsearch");
                var uri = new Uri(elasticConfig["Url"] ?? "http://localhost:9200");

                var settings = new ConnectionSettings(uri)
                    .DefaultIndex(elasticConfig["DefaultIndex"] ?? "listings")
                    .ThrowExceptions(false)
                    .RequestTimeout(TimeSpan.FromSeconds(30))
                    .DefaultMappingFor<Domain.Entities.SearchableListingDocument>(m => m
                        .IdProperty(p => p.Id)
                        .IndexName(elasticConfig["DefaultIndex"] ?? "listings"));

                var username = elasticConfig["Username"];
                var password = elasticConfig["Password"];
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    settings.BasicAuthentication(username, password);
                }

                return new ElasticClient(settings);
            });

            // --- Services de recherche ---
            services.AddScoped<ISearchRepository, ElasticsearchRepository>();
            services.AddScoped<ISearchQueryService, ElasticsearchQueryService>();
            services.AddScoped<IIndexManagementService, ElasticsearchIndexManagementService>();

            // --- EventBus (MassTransit + RabbitMQ) avec consumers ---
            services.AddEventBus(configuration, busConfig =>
            {
                busConfig.AddConsumer<ListingPublishedConsumer>();
                busConfig.AddConsumer<ListingUpdatedConsumer>();
                busConfig.AddConsumer<ListingDeactivatedConsumer>();
                busConfig.AddConsumer<MediaDeletedSearchConsumer>();
            });

            return services;
        }
    }

}
