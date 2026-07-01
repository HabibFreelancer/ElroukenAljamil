using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant une catégorie d'annonce.
    /// </summary>
    public record Category
    {
        public string Name { get; }

        public Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom de la catégorie est obligatoire.", nameof(name));

            Name = name.Trim();
        }

        public override string ToString() => Name;
    }
}
