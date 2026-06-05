namespace AutoHub.Models.Entities;

public class SparePartCompatibility : BaseEntity
{
    public int SparePartId { get; set; }

    public int VehicleNameId { get; set; }

    public int? VehicleVariantId { get; set; }

    public int? VehicleModelYearId { get; set; }

    public SparePart SparePart { get; set; } = null!;

    public VehicleName VehicleName { get; set; } = null!;

    public VehicleVariant? VehicleVariant { get; set; }

    public VehicleModelYear? VehicleModelYear { get; set; }
}
