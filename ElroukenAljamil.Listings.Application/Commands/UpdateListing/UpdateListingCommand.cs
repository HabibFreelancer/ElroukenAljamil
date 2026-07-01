using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.UpdateListing
{
    public record UpdateListingCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public string Category { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public List<string> ImageUrls { get; init; } = new();
    }
}
