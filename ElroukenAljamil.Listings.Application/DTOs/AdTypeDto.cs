namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record AdTypeDto
    {
        public int Id { get; init; }
        public int CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
    }

    public record CreateAdTypeRequest
    {
        public int CategoryId { get; init; }
        public string Label { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public int DisplayOrder { get; init; }
    }

    public record UpdateAdTypeRequest
    {
        public int CategoryId { get; init; }
        public string Label { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
    }
}
