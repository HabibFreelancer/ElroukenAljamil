using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.Security.Services
{
    /// <summary>
    /// Service injecté dans les couches Application pour accéder
    /// à l'utilisateur courant sans dépendre de HttpContext.
    /// </summary>
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Email { get; }
        string FullName { get; }
        IEnumerable<string> Roles { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }

}
