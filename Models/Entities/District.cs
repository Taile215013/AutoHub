namespace AutoHub.Models.Entities;

/// <summary>Quận / Huyện / Thị xã / TP thuộc tỉnh</summary>
public class District : BaseEntity
{
    /// <summary>Mã quận/huyện TMS, ví dụ "10105" = Quận Hoàn Kiếm</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ, ví dụ "Quận Hoàn Kiếm"</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã tỉnh BNV (FK)</summary>
    public string ProvinceCode { get; set; } = string.Empty;

    // Navigation
    public Province Province { get; set; } = null!;
    public ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
