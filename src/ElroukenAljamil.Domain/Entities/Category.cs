namespace ElroukenAljamil.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int? ParentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsLink { get; set; } = true;
    public bool ShowInDeposit { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public Menu? Menu { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}
