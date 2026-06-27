using MediatR;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;


namespace ElroukenAljamil.Listings.Application.Queries
{
    public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, ListingDto?>
    {
        private readonly IListingRepository _listingRepository;


        public GetListingByIdQueryHandler(IListingRepository listingRepository)
        {
            _listingRepository = listingRepository;
        }


        public async Task<ListingDto?> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
        {
            var listing = await _listingRepository.GetByIdAsync(request.ListingId, cancellationToken);


            if (listing is null)
                return null;


            return new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price.Amount,
                Currency = listing.Price.Currency,
                Status = listing.Status.ToString(),
                SellerId = listing.SellerId,
                CategoryId = listing.CategoryId,
                City = listing.Location.City,
                PostalCode = listing.Location.PostalCode,
                Images = listing.Images.Select(i => new ListingImageDto(i.Id, i.Url, i.DisplayOrder)).ToList(),
                CreatedAt = listing.CreatedAt
            };
        }
    }

}
