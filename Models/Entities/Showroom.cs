namespace AutoHub.Models.Entities;

/// <summary>
/// Chi nhánh / showroom của AutoHub.
/// Tọa độ dùng để hiển thị bản đồ Leaflet cho khách hàng tìm đường.
/// </summary>
public class Showroom : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    // ── Địa chỉ ───────────────────────────────────────────────────────
    public string? HouseNumber { get; set; }
    public string? StreetName  { get; set; }
    public string? Ward        { get; set; }
    public string? District    { get; set; }
    public string  City        { get; set; } = "Hồ Chí Minh";

    // ── Tọa độ địa lý ─────────────────────────────────────────────────
    /// <summary>
    /// Vĩ độ (latitude) — nhập tay hoặc geocode qua Nominatim.
    /// Ví dụ TP.HCM: 10.7769
    /// </summary>
    public double? Latitude  { get; set; }

    /// <summary>
    /// Kinh độ (longitude) — nhập tay hoặc geocode qua Nominatim.
    /// Ví dụ TP.HCM: 106.7009
    /// </summary>
    public double? Longitude { get; set; }

    // ── Trạng thái & hiển thị ─────────────────────────────────────────
    public bool IsActive { get; set; } = true;

    /// <summary>Giờ mở cửa, ví dụ "07:00 – 20:30".</summary>
    public string? OpeningHours { get; set; }

    /// <summary>Ảnh đại diện showroom.</summary>
    public string? ThumbnailImageUrl { get; set; }

    // ── Computed helper ────────────────────────────────────────────────

    /// <summary>Địa chỉ đầy đủ dạng string để hiển thị.</summary>
    public string FullAddress
        => string.Join(", ",
            new[] { HouseNumber, StreetName, Ward, District, City }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

    /// <summary>Kiểm tra đã có tọa độ chưa.</summary>
    public bool HasCoordinates => Latitude.HasValue && Longitude.HasValue;
}
