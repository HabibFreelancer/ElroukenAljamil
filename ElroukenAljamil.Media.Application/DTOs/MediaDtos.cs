using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Media.Application.DTOs
{
    public record MediaFileDto
    {
        public Guid Id { get; init; }
        public string OriginalFileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public long FileSize { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public string Status { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public Guid? ListingId { get; init; }
        public int SortOrder { get; init; }
        public MediaUrlsDto Urls { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }

    public record MediaUrlsDto
    {
        public string Original { get; init; } = string.Empty;
        public string? Thumbnail { get; init; }
        public string? Medium { get; init; }
        public string? Large { get; init; }
        public string? WebP { get; init; }
    }

    public record UploadResultDto
    {
        public Guid MediaId { get; init; }
        public string OriginalFileName { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
    }

    public record AssignMediaRequest
    {
        public Guid ListingId { get; init; }
        public List<Guid> MediaIds { get; init; } = new();
    }

    public record BatchUploadResultDto
    {
        public List<UploadResultDto> Uploaded { get; init; } = new();
        public List<UploadErrorDto> Errors { get; init; } = new();
    }

    public record UploadErrorDto
    {
        public string FileName { get; init; } = string.Empty;
        public string Error { get; init; } = string.Empty;
    }
}
