using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record CreateListingRequest
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public Guid CategoryId { get; init; }
        public string City { get; init; } = default!;
        public string PostalCode { get; init; } = default!;
        public string Country { get; init; } = "FR";
    }


    public record UpdateListingRequest
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public decimal? Price { get; init; }
    }

}
