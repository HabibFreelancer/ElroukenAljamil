using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElroukenAljamil.EventBus.Abstractions;
using ElroukenAljamil.EventBus.Events;

namespace ElroukenAljamil.Events.Abstractions
{
    /// <summary>
    /// Interface du bus d'événements pour la communication inter-services.
    /// </summary>
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;
        void Subscribe<T, THandler>() where T : IntegrationEvent where THandler : IIntegrationEventHandler<T>;
    }

}
