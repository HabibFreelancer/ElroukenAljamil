using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Security.Configuration
{
    /// <summary>
    /// Configuration JWT partagée — tous les services utilisent la même clé
    /// pour valider les tokens émis par Identity.Service.
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = "Marketplace.Identity";
        public string Audience { get; set; } = "Marketplace.Services";
        public int ExpirationMinutes { get; set; } = 60;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
