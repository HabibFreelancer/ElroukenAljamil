using AutoMapper;
using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Interfaces;
using MediatR;


namespace ElroukenAljamil.Listings.Application.Queries.GetListingById
{
    public class GetListingByIdQueryHandler : IRequestHandler<GetListingByIdQuery, Result<ListingDto>>
    {
        private readonly IListingRepository _repository;
        private readonly IMapper _mapper;

        public GetListingByIdQueryHandler(IListingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<ListingDto>> Handle(GetListingByIdQuery request, CancellationToken ct)
        {
            var listing = await _repository.GetByIdAsync(request.ListingId, ct);
            if (listing is null)
                return Result<ListingDto>.Failure($"Annonce {request.ListingId} introuvable.");

            // Incrémenter le compteur de vues
            listing.IncrementViewCount();
            await _repository.UpdateAsync(listing, ct);

            var dto = _mapper.Map<ListingDto>(listing);
            return Result<ListingDto>.Success(dto);
        }
    }

}
