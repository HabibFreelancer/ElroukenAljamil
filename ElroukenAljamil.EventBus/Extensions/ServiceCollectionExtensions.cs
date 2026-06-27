using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElroukenAljamil.EventBus.Configuration;

namespace ElroukenAljamil.EventBus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Enregistre MassTransit + RabbitMQ dans le conteneur DI.
        /// Chaque microservice appelle cette méthode et configure ses propres consumers.
        /// </summary>
        /// <param name="services">Collection de services</param>
        /// <param name="configuration">Configuration de l'application</param>
        /// <param name="configureConsumers">Action pour enregistrer les consumers spécifiques au service</param>
        /// <param name="serviceName">Nom du service (préfixe les queues)</param>
        public static IServiceCollection AddEventBus(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configureConsumers = null,
            string? serviceName = null)
        {
            var settings = configuration
                .GetSection(EventBusSettings.SectionName)
                .Get<EventBusSettings>() ?? new EventBusSettings();

            services.AddMassTransit(busConfig =>
            {
                // Enregistrement des consumers du service appelant
                configureConsumers?.Invoke(busConfig);

                busConfig.SetKebabCaseEndpointNameFormatter();

                busConfig.UsingRabbitMq((context, rabbitConfig) =>
                {
                    rabbitConfig.Host(settings.Host, settings.Port, settings.VirtualHost, hostConfig =>
                    {
                        hostConfig.Username(settings.Username);
                        hostConfig.Password(settings.Password);
                    });

                    // Retry policy globale
                    rabbitConfig.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(
                            settings.RetryCount,
                            TimeSpan.FromSeconds(settings.RetryIntervalSeconds));
                    });

                    // Configuration du prefetch
                    rabbitConfig.PrefetchCount = settings.PrefetchCount;

                    // Configurer automatiquement les endpoints des consumers enregistrés
                    rabbitConfig.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }

}
