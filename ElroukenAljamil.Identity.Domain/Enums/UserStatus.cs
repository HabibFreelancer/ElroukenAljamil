using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Identity.Domain.Enums
{
    public enum UserStatus
    {
        PendingVerification = 0,
        Active = 1,
        Deactivated = 2,
        Banned = 3
    }
}
