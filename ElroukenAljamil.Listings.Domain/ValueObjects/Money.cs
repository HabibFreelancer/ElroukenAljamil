using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant un montant monétaire.
    /// Immuable par conception.
    /// </summary>
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }


        public Money(decimal amount, string currency = "EUR")
        {
            Amount = amount;
            Currency = currency?.ToUpperInvariant() ?? "EUR";
        }
    }



}
