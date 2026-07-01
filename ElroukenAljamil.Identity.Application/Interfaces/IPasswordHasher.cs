using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Identity.Application.Interfaces
{
    /// <summary>
    /// Abstraction du hachage de mot de passe.
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
