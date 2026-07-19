using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Application.Interfaces
{
    /// <summary>
    /// Service qui gère la création et l'envoi des digest emails.
    /// Un digest regroupe les notifications non-lues d'un utilisateur
    /// en un seul email structuré par catégorie.
    /// </summary>
    public interface IDigestService
    {
        /// <summary>
        /// Traite tous les digests qui doivent être envoyés à l'instant T.
        /// Appelé périodiquement par le DigestBackgroundWorker.
        /// </summary>
        Task ProcessPendingDigestsAsync(CancellationToken ct = default);
    }
}
