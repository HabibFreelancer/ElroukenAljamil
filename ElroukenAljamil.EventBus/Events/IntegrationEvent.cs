using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.EventBus.Events
{
    /// <summary>
    /// Classe de base pour les événements d'intégration entre microservices.
    /// Utilisée pour la communication asynchrone via RabbitMQ.
    /// </summary>
    public abstract record IntegrationEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }

}
