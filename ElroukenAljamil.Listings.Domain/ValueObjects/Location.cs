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
    public record Location
    {
        public string City { get; }
        public double? Latitude { get; }
        public double? Longitude { get; }

        public Location(string city, double? latitude = null, double? longitude = null)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("La ville est obligatoire.", nameof(city));
            if (latitude.HasValue && (latitude < -90 || latitude > 90))
                throw new ArgumentException("La latitude doit être entre -90 et 90.", nameof(latitude));
            if (longitude.HasValue && (longitude < -180 || longitude > 180))
                throw new ArgumentException("La longitude doit être entre -180 et 180.", nameof(longitude));

            City = city.Trim();
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => Latitude.HasValue
            ? $"{City} ({Latitude:F4}, {Longitude:F4})"
            : City;
    }
}
