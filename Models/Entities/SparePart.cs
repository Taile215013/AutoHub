namespace AutoHub.Models.Entities;

public class SparePart : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int BrandId { get; set; }

    public string Category { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Brand Brand { get; set; } = null!;
}
