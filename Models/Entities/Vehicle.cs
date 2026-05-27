using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class Vehicle : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int BrandId { get; set; }

    public string? ImageUrl { get; set; }

    public string VehicleType { get; set; } = string.Empty;

    public string FuelType { get; set; } = string.Empty;

    public string Transmission { get; set; } = string.Empty;

    public decimal PurchasePrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public int? Quantity { get; set; }

    public string EngineType { get; set; } = string.Empty;

    public double? EngineCapacity { get; set; }

    public int SeatingCapacity { get; set; } = 1;

    public double? Weight { get; set; }

    public string? BodyStyle { get; set; }

    public Brand Brand { get; set; } = null!;

    public ICollection<VehicleColor> Colors { get; set; } = new List<VehicleColor>();
}
