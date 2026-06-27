using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Common.Exceptions
{
    /// <summary>
    /// Exception métier levée par le domaine.
    /// </summary>
    public class DomainException : Exception
    {
        public string Code { get; }

        public DomainException(string message, string code = "DOMAIN_ERROR")
            : base(message)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Entité introuvable.
    /// </summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} avec l'identifiant '{id}' introuvable.", "NOT_FOUND")
        {
        }
    }

    /// <summary>
    /// Accès refusé.
    /// </summary>
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message = "Accès refusé.")
            : base(message, "FORBIDDEN")
        {
        }
    }

    /// <summary>
    /// Conflit métier (état incohérent).
    /// </summary>
    public class ConflictException : DomainException
    {
        public ConflictException(string message)
            : base(message, "CONFLICT")
        {
        }
    }

}
