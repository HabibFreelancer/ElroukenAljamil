using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant une localisation géographique.
    /// </summary>
    public record Address
    {
        public string City { get; init; } = default!;
        public string PostalCode { get; init; } = default!;
        public string Country { get; init; } = default!;
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }


        public Address(string city, string postalCode, string country, double? latitude = null, double? longitude = null)
        {
            City = city;
            PostalCode = postalCode;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }
    }

}
