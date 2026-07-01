using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;


namespace ElroukenAljamil.Listings.Application.Commands.CreateListing
{
    public record CreateListingCommand : IRequest<Result<Guid>>
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public string Category { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public List<string> ImageUrls { get; init; } = new();
        public Guid SellerId { get; internal set; }
    }

}
