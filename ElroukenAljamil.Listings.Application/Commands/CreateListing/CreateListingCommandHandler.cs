using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Interfaces;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.ValueObjects;
using MediatR;


namespace ElroukenAljamil.Listings.Application.Commands.CreateListing
{
    public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Result<Guid>>
    {
        private readonly IListingRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public CreateListingCommandHandler(
            IListingRepository repository,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result<Guid>> Handle(CreateListingCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<Guid>.Failure("Utilisateur non authentifié.");

            var category = new Category(request.Category);
            var location = new Location(request.City, request.Latitude, request.Longitude);
            var price = new Money(request.Price, request.Currency);

            var listing = Listing.Create(
                title: request.Title,
                description: request.Description,
                price: price,
                category: category,
                location: location,
                sellerId: _currentUser.UserId,
                sellerName: _currentUser.FullName ?? "Utilisateur",
                imageUrls: request.ImageUrls);

            await _repository.AddAsync(listing, ct);

            await _eventBus.PublishAsync(new ListingPublishedEvent
            {
                ListingId = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price.Amount,
                Currency = listing.Price.Currency,
                Category = listing.Category.Name,
                City = listing.Location.City,
                Latitude = listing.Location.Latitude,
                Longitude = listing.Location.Longitude,
                SellerId = listing.SellerId,
                SellerName = listing.SellerName,
                ImageUrls = listing.ImageUrls,
                PublishedAt = DateTime.UtcNow
            }, ct);

            return Result<Guid>.Success(listing.Id);
        }
    }

}
