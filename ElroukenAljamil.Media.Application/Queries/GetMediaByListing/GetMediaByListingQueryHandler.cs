using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Media.Application.DTOs;
using ElroukenAljamil.Media.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Media.Application.Queries.GetMediaByListing
{
    public class GetMediaByListingQueryHandler : IRequestHandler<GetMediaByListingQuery, Result<List<MediaFileDto>>>
    {
        private readonly IMediaFileRepository _repository;
        private readonly string _storageBaseUrl;

        public GetMediaByListingQueryHandler(
            IMediaFileRepository repository,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _repository = repository;
            _storageBaseUrl = configuration["MinIO:PublicUrl"] ?? "http://localhost:9000";
        }

        public async Task<Result<List<MediaFileDto>>> Handle(GetMediaByListingQuery request, CancellationToken ct)
        {
            var mediaFiles = await _repository.GetByListingIdAsync(request.ListingId, ct);

            var dtos = mediaFiles
                .OrderBy(m => m.SortOrder)
                .Select(m => new MediaFileDto
                {
                    Id = m.Id,
                    OriginalFileName = m.OriginalFileName,
                    ContentType = m.ContentType,
                    FileSize = m.FileSize,
                    Width = m.Dimensions.Width,
                    Height = m.Dimensions.Height,
                    Status = m.Status.ToString(),
                    OwnerId = m.OwnerId,
                    ListingId = m.ListingId,
                    SortOrder = m.SortOrder,
                    Urls = new MediaUrlsDto
                    {
                        Original = $"{_storageBaseUrl}/{m.BucketName}/{m.StoragePath}",
                        Thumbnail = m.Variants.ThumbnailPath != null
                            ? $"{_storageBaseUrl}/{m.BucketName}/{m.Variants.ThumbnailPath}" : null,
                        Medium = m.Variants.MediumPath != null
                            ? $"{_storageBaseUrl}/{m.BucketName}/{m.Variants.MediumPath}" : null,
                        Large = m.Variants.LargePath != null
                            ? $"{_storageBaseUrl}/{m.BucketName}/{m.Variants.LargePath}" : null,
                        WebP = m.Variants.WebPPath != null
                            ? $"{_storageBaseUrl}/{m.BucketName}/{m.Variants.WebPPath}" : null
                    },
                    CreatedAt = m.CreatedAt
                })
                .ToList();

            return Result<List<MediaFileDto>>.Success(dtos);
        }
    }
}
