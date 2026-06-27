using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.Exceptions
{
    /// <summary>
    /// Exception spécifique aux règles métier du domaine Listings.
    /// </summary>
    public class ListingDomainException : Exception
    {
        public ListingDomainException(string message) : base(message) { }
        public ListingDomainException(string message, Exception inner) : base(message, inner) { }
    }

}
