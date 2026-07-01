using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeactivateListing
{
    public class DeactivateListingCommandHandler : IRequestHandler<DeactivateListingCommand, Result>
    {
        private readonly IListingRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public DeactivateListingCommandHandler(
            IListingRepository repository,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(DeactivateListingCommand request, CancellationToken ct)
        {
            var listing = await _repository.GetByIdAsync(request.Id, ct);
            if (listing is null)
                return Result.Failure($"Annonce {request.Id} introuvable.");

            if (listing.SellerId != _currentUser.UserId)
                return Result.Failure("Vous n'êtes pas autorisé à désactiver cette annonce.");

            listing.Deactivate();
            await _repository.UpdateAsync(listing, ct);

            await _eventBus.PublishAsync(new ListingDeactivatedEvent
            {
                ListingId = listing.Id,
                SellerId = listing.SellerId,
                DeactivatedAt = DateTime.UtcNow
            }, ct);

            return Result.Success();
        }
    }
}
