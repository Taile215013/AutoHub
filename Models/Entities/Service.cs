namespace AutoHub.Models.Entities;

public class Service : BaseEntity
{
    public string ServiceName { get; set; } = string.Empty;

    public decimal? BasePrice { get; set; }

    public bool IsActive { get; set; } = true;

    public string VehicleType { get; set; } = string.Empty;

    public bool RequiresQuote { get; set; } = false;
}
