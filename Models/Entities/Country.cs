using System.Collections.Generic;

namespace AutoHub.Models.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
