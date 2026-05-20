namespace AutoHub.Models.Entities;

public class VehicleColor : BaseEntity
{
    public int VehicleId { get; set; }

    public string ColorName { get; set; } = string.Empty;

    public Vehicle Vehicle { get; set; } = null!;
}
