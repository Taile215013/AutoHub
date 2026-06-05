using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AutoHub.Models.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Gender    { get; set; } = string.Empty;

    /// <summary>Ngày sinh — dùng tính tuổi tự động</summary>
    public DateTime DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Email       { get; set; }

    /// <summary>Căn cước công dân (CCCD / ID)</summary>
    public string? NationalId { get; set; }

    // ── Địa chỉ phân cấp ──────────────────────────────────────────────
    public string? HouseNumber { get; set; }
    public string? StreetName  { get; set; }
    public string? Ward        { get; set; }
    public string? District    { get; set; }
    public string  City        { get; set; } = "Hồ Chí Minh";

    // ── Thể trạng ─────────────────────────────────────────────────────
    /// <summary>Chiều cao (cm)</summary>
    public int? HeightCm { get; set; }

    /// <summary>Cân nặng (kg)</summary>
    public decimal? WeightKg { get; set; }

    // ── Công việc ─────────────────────────────────────────────────────
    /// <summary>Chức vụ — Code từ SystemDictionaries type "EmployeePosition"</summary>
    public string  Position   { get; set; } = string.Empty;

    /// <summary>Lương cứng (VNĐ/tháng)</summary>
    public decimal BaseSalary { get; set; }

    public bool    IsActive   { get; set; } = true;
    public string? Notes      { get; set; }

    // ── Ảnh ───────────────────────────────────────────────────────────
    /// <summary>Ảnh đại diện / profile (1 ảnh duy nhất)</summary>
    public string? ThumbnailImageUrl { get; set; }

    /// <summary>Nhiều ảnh kèm theo — lưu JSON array URL</summary>
    public string? ImageUrl { get; set; }

    // ── Computed helpers (không ánh xạ DB) ────────────────────────────

    /// <summary>Tuổi tính theo ngày hôm nay.</summary>
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age   = today.Year - DateOfBirth.Year;
            if (today < DateOfBirth.AddYears(age)) age--;
            return age;
        }
    }

    /// <summary>
    /// Deserialize ImageUrl JSON thành danh sách URL.
    /// Trả về list rỗng nếu null hoặc JSON lỗi — không throw exception.
    /// </summary>
    public List<string> GetImageList()
    {
        if (string.IsNullOrEmpty(ImageUrl)) return new();
        try   { return JsonSerializer.Deserialize<List<string>>(ImageUrl) ?? new(); }
        catch { return new(); }
    }
}
