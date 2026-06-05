using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string CategoryType { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    public ProductCategory? ParentCategory { get; set; }

    public ICollection<ProductCategory> ChildCategories { get; set; } = new List<ProductCategory>();

    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
