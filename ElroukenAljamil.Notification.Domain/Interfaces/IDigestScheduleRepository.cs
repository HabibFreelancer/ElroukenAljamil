using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    /// <summary>
    /// Repository pour les configurations de digest.
    /// </summary>
    public interface IDigestScheduleRepository : IRepository<DigestSchedule>
    {
        Task<DigestSchedule?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<IReadOnlyList<DigestSchedule>> GetActiveSchedulesAsync(CancellationToken ct = default);
    }


}
