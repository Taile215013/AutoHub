namespace AutoHub.Models.Entities;

public class SparePart : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int BrandId { get; set; }

    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailImageUrl { get; set; }

    public string? AdditionalImages { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Status { get; set; } = "InStock";

    public int? ManufactureYear { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Brand Brand { get; set; } = null!;

    public ProductCategory? CategoryMaster { get; set; }

    public ICollection<SparePartCompatibility> Compatibilities { get; set; } = new List<SparePartCompatibility>();
}
