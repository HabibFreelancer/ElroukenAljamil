using MediatR;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Domain.ValueObjects;
using ElroukenAljamil.Common.Interfaces;


namespace ElroukenAljamil.Listings.Application.Commands
{
    public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, ListingDto>
    {
        private readonly IListingRepository _listingRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CreateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
        {
            _listingRepository = listingRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<ListingDto> Handle(CreateListingCommand request, CancellationToken cancellationToken)
        {
            var price = new Money(request.Price, request.Currency);
            var location = new Address(request.City, request.PostalCode, request.Country);


            var listing = Listing.Create(
                title: request.Title,
                description: request.Description,
                price: price,
                sellerId: request.SellerId,
                categoryId: request.CategoryId,
                location: location
            );


            await _listingRepository.AddAsync(listing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


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
                CreatedAt = listing.CreatedAt
            };
        }
    }

}
