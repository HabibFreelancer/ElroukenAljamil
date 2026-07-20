namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record MenuDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string? Icon { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
    }

    public record CategoryDto
    {
        public int Id { get; init; }
        public int MenuId { get; init; }
        public string MenuName { get; init; } = string.Empty;
        public string? MenuIcon { get; init; }
        public int? ParentCategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public bool IsLink { get; init; }
        public bool ShowInDeposit { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
        public List<CategoryDto> SubCategories { get; init; } = new();
    }

    public record CreateCategoryRequest
    {
        public int MenuId { get; init; }
        public int? ParentCategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public bool IsLink { get; init; } = true;
        public bool ShowInDeposit { get; init; } = true;
        public int DisplayOrder { get; init; }
    }

    public record UpdateCategoryRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public int? ParentCategoryId { get; init; }
        public bool IsLink { get; init; } = true;
        public bool ShowInDeposit { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
    }

    public record CreateMenuRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string? Icon { get; init; }
        public int DisplayOrder { get; init; }
    }

    public record UpdateMenuRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string? Icon { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
    }
}
