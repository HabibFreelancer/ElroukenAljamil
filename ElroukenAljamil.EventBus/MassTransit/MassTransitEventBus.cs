using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using MassTransit;

namespace ElroukenAljamil.BuildingBlocks.EventBus.MassTransit
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
            => _publishEndpoint.Publish(@event, cancellationToken);

        public void Subscribe<T, THandler>()
            where T : IntegrationEvent
            where THandler : IIntegrationEventHandler<T>
        {
            // MassTransit gère les subscriptions via la configuration des consumers au démarrage
        }
    }
}
