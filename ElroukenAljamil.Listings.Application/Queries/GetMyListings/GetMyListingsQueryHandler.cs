using AutoMapper;
using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetMyListings
{
    public class GetMyListingsQueryHandler : IRequestHandler<GetMyListingsQuery, Result<List<ListingSummaryDto>>>
    {
        private readonly IListingRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public GetMyListingsQueryHandler(
            IListingRepository repository,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _repository = repository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<Result<List<ListingSummaryDto>>> Handle(
            GetMyListingsQuery request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<List<ListingSummaryDto>>.Failure("Utilisateur non authentifié.");

            var listings = await _repository.GetBySellerIdAsync(_currentUser.UserId, ct);
            var dtos = _mapper.Map<List<ListingSummaryDto>>(listings);

            return Result<List<ListingSummaryDto>>.Success(dtos);
        }
    }
}
