namespace AutoHub.Models.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }

    public string ProductType { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public Order Order { get; set; } = null!;
}
