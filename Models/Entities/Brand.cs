using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }

    public bool IsVehicleBrand { get; set; } = false;

    public bool IsPartBrand { get; set; } = false;

    public bool IsToyBrand { get; set; } = false;

    public Country Country { get; set; } = null!;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
}