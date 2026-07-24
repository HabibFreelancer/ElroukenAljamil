using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Domain.Entities
{
    public class AdContext
    {
        public string Category { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public Dictionary<string, object> RawData { get; set; } = new();
    }
}
