namespace AutoHub.Models.Entities;

/// <summary>Phường / Xã / Thị trấn</summary>
public class Ward : BaseEntity
{
    /// <summary>Mã phường/xã mới chính thống, ví dụ "10105001" = Phường Hoàn Kiếm</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ, ví dụ "Phường Hoàn Kiếm"</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã quận/huyện TMS (FK)</summary>
    public string DistrictCode { get; set; } = string.Empty;

    // Navigation
    public District District { get; set; } = null!;
}
