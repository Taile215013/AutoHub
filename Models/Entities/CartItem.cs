namespace AutoHub.Models.Entities;

public class CartItem : BaseEntity
{
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty; // "vehicle" or "service"
    
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
