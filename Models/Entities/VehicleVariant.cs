using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class VehicleVariant : BaseEntity
{
    public int VehicleNameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? EngineType { get; set; }

    public double? EngineCapacity { get; set; }

    public VehicleName VehicleName { get; set; } = null!;

    public ICollection<VehicleModelYear> ModelYears { get; set; } = new List<VehicleModelYear>();

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public ICollection<SparePartCompatibility> SparePartCompatibilities { get; set; } = new List<SparePartCompatibility>();
}
