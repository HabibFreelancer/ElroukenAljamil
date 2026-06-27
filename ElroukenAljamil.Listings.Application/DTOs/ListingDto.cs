using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record ListingDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
        public decimal Price { get; init; }
        public string Currency { get; init; } = default!;
        public string Status { get; init; } = default!;
        public Guid SellerId { get; init; }
        public Guid CategoryId { get; init; }
        public string City { get; init; } = default!;
        public string PostalCode { get; init; } = default!;
        public List<ListingImageDto> Images { get; init; } = new();
        public DateTime CreatedAt { get; init; }
    }
    public record ListingImageDto(Guid Id, string Url, int DisplayOrder);

}
