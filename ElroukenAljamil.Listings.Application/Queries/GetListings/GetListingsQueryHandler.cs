using AutoMapper;
using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Interfaces;
using ElroukenAljamil.Listings.Domain.Enums;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetListings
{
    public class GetListingsQueryHandler : IRequestHandler<GetListingsQuery, Result<BuildingBlocks.Common.Results.PagedResult<ListingSummaryDto>>>
    {
        private readonly IListingRepository _repository;
        private readonly IMapper _mapper;

        public GetListingsQueryHandler(IListingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<BuildingBlocks.Common.Results.PagedResult<ListingSummaryDto>>> Handle(
            GetListingsQuery request, CancellationToken ct)
        {
            ListingStatus? status = null;
            if (!string.IsNullOrEmpty(request.Status) &&
                Enum.TryParse<ListingStatus>(request.Status, true, out var parsedStatus))
            {
                status = parsedStatus;
            }

            var (items, totalCount) = await _repository.GetPagedAsync(
                page: request.Page,
                pageSize: request.PageSize,
                status: status,
                category: request.Category,
                sellerId: request.SellerId,
                ct: ct);

            var dtos = _mapper.Map<List<ListingSummaryDto>>(items);

            var pagedResult = new BuildingBlocks.Common.Results.PagedResult<ListingSummaryDto>(
                     dtos,
                     totalCount,
                     request.Page,
                     request.PageSize
                );

            return Result<BuildingBlocks.Common.Results.PagedResult<ListingSummaryDto>>.Success(pagedResult);
        }
    }

}
