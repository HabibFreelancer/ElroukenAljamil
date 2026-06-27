using MediatR;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Domain.Exceptions;
using ElroukenAljamil.Common.Interfaces;


namespace ElroukenAljamil.Listings.Application.Commands
{
    public class PublishListingCommandHandler : IRequestHandler<PublishListingCommand, Unit>
    {
        private readonly IListingRepository _listingRepository;
        private readonly IUnitOfWork _unitOfWork;


        public PublishListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
        {
            _listingRepository = listingRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Unit> Handle(PublishListingCommand request, CancellationToken cancellationToken)
        {
            var listing = await _listingRepository.GetByIdAsync(request.ListingId, cancellationToken)
                ?? throw new ListingDomainException($"Annonce {request.ListingId} introuvable.");


            listing.Publish();


            await _listingRepository.UpdateAsync(listing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return Unit.Value;
        }
    }

}
