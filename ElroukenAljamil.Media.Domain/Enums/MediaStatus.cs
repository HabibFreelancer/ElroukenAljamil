using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Media.Domain.Enums
{
    public enum MediaStatus
    {
        Uploaded = 0,
        Processing = 1,
        Processed = 2,
        Failed = 3,
        MarkedForDeletion = 4
    }
}
