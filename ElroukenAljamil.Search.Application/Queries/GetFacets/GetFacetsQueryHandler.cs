using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using ElroukenAljamil.Search.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.GetFacets
{
    public class GetFacetsQueryHandler : IRequestHandler<GetFacetsQuery, Result<FacetsResponseDto>>
    {
        private readonly ISearchQueryService _searchQueryService;

        public GetFacetsQueryHandler(ISearchQueryService searchQueryService)
        {
            _searchQueryService = searchQueryService;
        }

        public async Task<Result<FacetsResponseDto>> Handle(GetFacetsQuery request, CancellationToken ct)
        {
            var categories = await _searchQueryService.GetCategoryFacetsAsync(request.Query, ct);
            var priceRange = await _searchQueryService.GetPriceRangeAsync(request.Category, ct);

            var dto = new FacetsResponseDto
            {
                Categories = categories.Select(c => new CategoryFacetDto(c.Name, c.Count)).ToList(),
                PriceRange = new PriceRangeDto(priceRange.Min, priceRange.Max, priceRange.Average)
            };

            return Result<FacetsResponseDto>.Success(dto);
        }
    }
}
