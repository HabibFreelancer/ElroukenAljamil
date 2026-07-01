using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ElroukenAljamil.BuildingBlocks.Common.Domain
{
    /// <summary>
    /// Marqueur pour les événements de domaine (intra-service via MediatR).
    /// </summary>
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
    /// <summary>
    /// Implémentation de base d'un domain event.
    /// </summary>
    public abstract record BaseDomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
