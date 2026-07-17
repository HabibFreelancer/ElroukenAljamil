using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using ElroukenAljamil.Search.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.SearchListings
{
    public class SearchListingsQueryHandler : IRequestHandler<SearchListingsQuery, Result<SearchResultDto>>
    {
        private readonly ISearchQueryService _searchQueryService;

        public SearchListingsQueryHandler(ISearchQueryService searchQueryService)
        {
            _searchQueryService = searchQueryService;
        }

        public async Task<Result<SearchResultDto>> Handle(SearchListingsQuery request, CancellationToken ct)
        {
            var criteria = new SearchCriteria
            {
                Query = request.Query,
                Category = request.Category,
                City = request.City,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                RadiusKm = request.RadiusKm,
                SellerId = request.SellerId,
                SortBy = request.SortBy,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var result = await _searchQueryService.SearchAsync(criteria, ct);

            var dto = new SearchResultDto
            {
                Items = result.Items.Select(item => new SearchListingItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description.Length > 200
                        ? item.Description[..200] + "..."
                        : item.Description,
                    Price = item.Price,
                    Currency = item.Currency,
                    Category = item.Category,
                    City = item.City,
                    ThumbnailUrl = item.ThumbnailUrl,
                    SellerName = item.SellerName,
                    PublishedAt = item.PublishedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage,
                SearchDurationMs = result.SearchDuration.TotalMilliseconds
            };

            return Result<SearchResultDto>.Success(dto);
        }
    }


}
