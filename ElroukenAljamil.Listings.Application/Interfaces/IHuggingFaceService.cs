using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Application.Interfaces
{
    public interface IHuggingFaceService
    {
        Task<string?> CallHuggingFaceAsync(string prompt, CancellationToken ct);
    }
}
