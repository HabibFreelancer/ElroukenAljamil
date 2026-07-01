using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Identity.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant un numéro de téléphone validé.
    /// </summary>
    public record PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Le numéro de téléphone ne peut pas être vide.");

            // Nettoyage basique : garder uniquement les chiffres et le +
            var cleaned = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());

            if (cleaned.Length < 8 || cleaned.Length > 15)
                throw new ArgumentException("Le numéro de téléphone doit contenir entre 8 et 15 chiffres.");

            Value = cleaned;
        }

        public override string ToString() => Value;
    }
}
