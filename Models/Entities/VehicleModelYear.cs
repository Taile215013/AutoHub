using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class VehicleModelYear : BaseEntity
{
    public int VehicleVariantId { get; set; }

    public int Year { get; set; }

    public VehicleVariant VehicleVariant { get; set; } = null!;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public ICollection<SparePartCompatibility> SparePartCompatibilities { get; set; } = new List<SparePartCompatibility>();
}
