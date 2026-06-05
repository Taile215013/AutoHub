using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class VehicleName : BaseEntity
{
    public int BrandId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string? BodyStyle { get; set; }

    public Brand Brand { get; set; } = null!;

    public ICollection<VehicleVariant> Variants { get; set; } = new List<VehicleVariant>();

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public ICollection<SparePartCompatibility> SparePartCompatibilities { get; set; } = new List<SparePartCompatibility>();
}
