using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Identity.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant une adresse postale.
    /// </summary>
    public record Address
    {
        public string Street { get; }
        public string City { get; }
        public string ZipCode { get; }
        public string Country { get; }
        public string? State { get; }

        public Address(string street, string city, string zipCode, string country, string? state = null)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("La rue est obligatoire.");
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("La ville est obligatoire.");
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("Le code postal est obligatoire.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Le pays est obligatoire.");

            Street = street.Trim();
            City = city.Trim();
            ZipCode = zipCode.Trim();
            Country = country.Trim();
            State = state?.Trim();
        }

        public override string ToString() => $"{Street}, {ZipCode} {City}, {Country}";
    }
}
