using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Interfaces;
using ElroukenAljamil.Listings.Domain.ValueObjects;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.UpdateListing
{
    public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand, Result>
    {
        private readonly IListingRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public UpdateListingCommandHandler(
            IListingRepository repository,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(UpdateListingCommand request, CancellationToken ct)
        {
            var listing = await _repository.GetByIdAsync(request.Id, ct);
            if (listing is null)
                return Result.Failure($"Annonce {request.Id} introuvable.");

            if (listing.SellerId != _currentUser.UserId)
                return Result.Failure("Vous n'êtes pas autorisé à modifier cette annonce.");

            var newPrice = new Money(request.Price, request.Currency);
            var newCategory = new Category(request.Category);
            var newLocation = new Location(request.City, request.Latitude, request.Longitude);

            listing.Update(
                title: request.Title,
                description: request.Description,
                price: newPrice,
                category: newCategory,
                location: newLocation,
                imageUrls: request.ImageUrls);

            await _repository.UpdateAsync(listing, ct);

            await _eventBus.PublishAsync(new ListingUpdatedEvent
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
                ImageUrls = listing.ImageUrls,
                UpdatedAt = DateTime.UtcNow
            }, ct);

            return Result.Success();
        }
    }

}
