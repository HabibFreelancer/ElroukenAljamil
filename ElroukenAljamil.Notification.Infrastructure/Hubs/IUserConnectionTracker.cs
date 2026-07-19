using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Infrastructure.Hubs
{
    /// <summary>
    /// Interface pour le suivi des connexions utilisateur.
    /// Permet de savoir quels utilisateurs sont en ligne et sur combien de devices.
    /// </summary>
    public interface IUserConnectionTracker
    {
        /// <summary>
        /// Enregistre une nouvelle connexion pour un utilisateur.
        /// </summary>
        Task AddConnectionAsync(Guid userId, string connectionId);

        /// <summary>
        /// Retire une connexion d'un utilisateur.
        /// </summary>
        Task RemoveConnectionAsync(Guid userId, string connectionId);

        /// <summary>
        /// Vérifie si un utilisateur a au moins une connexion active.
        /// </summary>
        Task<bool> IsOnlineAsync(Guid userId);

        /// <summary>
        /// Récupère toutes les connexions actives d'un utilisateur.
        /// </summary>
        Task<IReadOnlyList<string>> GetConnectionsAsync(Guid userId);

        /// <summary>
        /// Récupère le nombre total d'utilisateurs connectés.
        /// </summary>
        Task<int> GetOnlineCountAsync();
    }
}
