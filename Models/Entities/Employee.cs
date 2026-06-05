namespace AutoHub.Models.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    /// <summary>Ngày sinh — dùng tính tuổi tự động</summary>
    public DateTime DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    /// <summary>Căn cước công dân (CCCD / ID)</summary>
    public string? NationalId { get; set; }

    // ── Địa chỉ phân cấp ──────────────────────────────────────────────
    public string? HouseNumber { get; set; }
    public string? StreetName { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string City { get; set; } 

    // ── Thể trạng ─────────────────────────────────────────────────────
    /// <summary>Chiều cao (cm)</summary>
    public int? HeightCm { get; set; }

    /// <summary>Cân nặng (kg)</summary>
    public decimal? WeightKg { get; set; }

    // ── Công việc ─────────────────────────────────────────────────────
    /// <summary>Chức vụ — Code từ SystemDictionaries type "EmployeePosition"</summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>Lương cứng (VNĐ/tháng)</summary>
    public decimal BaseSalary { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }

    // ── Ảnh ───────────────────────────────────────────────────────────
    /// <summary>Ảnh đại diện / profile (1 ảnh duy nhất)</summary>
    public string? ThumbnailImageUrl { get; set; }

    /// <summary>Nhiều ảnh kèm theo — lưu JSON array URL</summary>
    public string? ImageUrl { get; set; }
}
