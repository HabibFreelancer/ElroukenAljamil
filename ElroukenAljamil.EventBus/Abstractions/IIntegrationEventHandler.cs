using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TEvent> where TEvent : IntegrationEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }

}
