using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant un montant monétaire.
    /// </summary>
    public record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Le montant ne peut pas être négatif.", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new ArgumentException("La devise doit être un code ISO 4217 (3 caractères).", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public override string ToString() => $"{Amount:F2} {Currency}";
    }



}
