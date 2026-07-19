using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Notification.Domain.Enums
{
    /// <summary>
    /// Statuts du cycle de vie d'une notification.
    /// Transitions possibles :
    ///   Pending → Sent (envoi réussi)
    ///   Pending → Failed (après 3 tentatives échouées)
    ///   Sent → Read (l'utilisateur a lu, uniquement pour InApp)
    /// </summary>
    public enum NotificationStatus
    {
        Pending = 0,    // En attente d'envoi (ou en cours de retry)
        Sent = 1,       // Envoyée avec succès au provider
        Failed = 2,     // Échec définitif après toutes les tentatives
        Read = 3        // Lue par l'utilisateur (uniquement pour le canal InApp)
    }
}
