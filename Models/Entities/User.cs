namespace AutoHub.Models.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? HouseNumber { get; set; }

    public string StreetName { get; set; } = string.Empty;

    public string Ward { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string City { get; set; } = "Hồ Chí Minh";

    public string RankLevel { get; set; } = "Bronze";

    // ── Tọa độ địa lý (Nominatim geocode từ địa chỉ) ──────────────────
    /// <summary>Vĩ độ — null cho đến khi geocode.</summary>
    public double? Latitude  { get; set; }

    /// <summary>Kinh độ — null cho đến khi geocode.</summary>
    public double? Longitude { get; set; }
}
