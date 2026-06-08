namespace AutoHub.Models.Entities;

/// <summary>Tỉnh / Thành phố trực thuộc Trung ương (63 đơn vị)</summary>
public class Province : BaseEntity
{
    /// <summary>Mã tỉnh BNV chính thống, ví dụ "01" = Hà Nội, "79" = TP.HCM</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên đầy đủ, ví dụ "Thành phố Hà Nội"</summary>
    public string Name { get; set; } = string.Empty;

    // Navigation
    public ICollection<District> Districts { get; set; } = new List<District>();
}
