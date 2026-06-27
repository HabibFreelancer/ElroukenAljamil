using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Common.Domain
{
    /// <summary>
    /// Racine d'agrégat : seule entrée pour modifier un agrégat.
    /// </summary>
    public abstract class AggregateRoot : BaseEntity
    {
        public int Version { get; protected set; }

        protected void IncrementVersion()
        {
            Version++;
            SetUpdated();
        }
    }
}
