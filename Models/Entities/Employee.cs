using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AutoHub.Models.Entities;

/// <summary>
/// Ca làm việc cố định của công ty.
/// Dùng string constant để tránh magic string rải khắp code.
/// </summary>
public static class WorkShift
{
    public const string Morning   = "Sáng";    // 07:00 – 11:30
    public const string Afternoon = "Chiều";   // 13:00 – 17:30
    public const string Evening   = "Tối";     // 18:00 – 20:30
    public const string DayOff    = "Nghỉ";

    /// <summary>Nhãn hiển thị kèm giờ.</summary>
    public static string Label(string shift) => shift switch
    {
        Morning   => "Ca Sáng (07:00 – 11:30)",
        Afternoon => "Ca Chiều (13:00 – 17:30)",
        Evening   => "Ca Tối (18:00 – 20:30)",
        DayOff    => "Nghỉ",
        _         => shift
    };

    public static readonly string[] All = [Morning, Afternoon, Evening, DayOff];
}

/// <summary>Lịch làm việc 1 tuần — key: tên ngày tiếng Việt, value: tên ca.</summary>
public sealed class WeekSchedule
{
    public string Monday    { get; set; } = WorkShift.DayOff;
    public string Tuesday   { get; set; } = WorkShift.DayOff;
    public string Wednesday { get; set; } = WorkShift.DayOff;
    public string Thursday  { get; set; } = WorkShift.DayOff;
    public string Friday    { get; set; } = WorkShift.DayOff;
    public string Saturday  { get; set; } = WorkShift.DayOff;
    public string Sunday    { get; set; } = WorkShift.DayOff;

    /// <summary>Trả về danh sách (tên ngày VN, tên ca) để render bảng dễ hơn.</summary>
    public IEnumerable<(string Day, string DayVn, string Shift)> ToRows() =>
    [
        ("Monday",    "Thứ Hai",  Monday),
        ("Tuesday",   "Thứ Ba",   Tuesday),
        ("Wednesday", "Thứ Tư",   Wednesday),
        ("Thursday",  "Thứ Năm",  Thursday),
        ("Friday",    "Thứ Sáu",  Friday),
        ("Saturday",  "Thứ Bảy",  Saturday),
        ("Sunday",    "Chủ Nhật", Sunday),
    ];
}

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Gender    { get; set; } = string.Empty;

    /// <summary>Ngày sinh.</summary>
    public DateTime  DateOfBirth { get; set; }

    /// <summary>Ngày bắt đầu làm việc (có thể null nếu chưa xác định).</summary>
    public DateTime? StartDate   { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Email       { get; set; }

    /// <summary>Căn cước công dân (CCCD / ID).</summary>
    public string? NationalId  { get; set; }

    // ── Địa chỉ phân cấp ──────────────────────────────────────────────
    public string? HouseNumber { get; set; }
    public string? StreetName  { get; set; }
    public string? Ward        { get; set; }
    public string? District    { get; set; }
    public string  City        { get; set; } = "Hồ Chí Minh";

    // ── Thể trạng ─────────────────────────────────────────────────────
    /// <summary>Chiều cao (cm).</summary>
    public int?     HeightCm { get; set; }

    /// <summary>Cân nặng (kg).</summary>
    public decimal? WeightKg { get; set; }

    // ── Công việc ─────────────────────────────────────────────────────
    /// <summary>Chức vụ — Code từ SystemDictionaries type "EmployeePosition".</summary>
    public string  Position   { get; set; } = string.Empty;

    /// <summary>Lương cứng (VNĐ/tháng).</summary>
    public decimal BaseSalary { get; set; }

    public bool    IsActive   { get; set; } = true;
    public string? Notes      { get; set; }

    /// <summary>
    /// Lịch làm việc theo tuần — lưu dạng JSON.
    /// Dùng <see cref="GetSchedule"/> và <see cref="SetSchedule"/> để đọc/ghi.
    /// </summary>
    public string? WorkScheduleJson { get; set; }

    // ── Ảnh ───────────────────────────────────────────────────────────
    /// <summary>Ảnh đại diện / profile (1 ảnh duy nhất).</summary>
    public string? ThumbnailImageUrl { get; set; }

    /// <summary>Nhiều ảnh kèm theo — lưu JSON array URL.</summary>
    public string? ImageUrl { get; set; }

    // ── Tọa độ địa lý (Nominatim geocode từ địa chỉ nhà) ──────────────
    /// <summary>Vĩ độ — null cho đến khi geocode.</summary>
    public double? Latitude  { get; set; }

    /// <summary>Kinh độ — null cho đến khi geocode.</summary>
    public double? Longitude { get; set; }

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

    /// <summary>Số năm làm việc (null nếu StartDate chưa có).</summary>
    public double? YearsOfService
        => StartDate.HasValue
            ? Math.Round((DateTime.Today - StartDate.Value).TotalDays / 365.25, 1)
            : null;

    /// <summary>Deserialize <see cref="WorkScheduleJson"/> thành <see cref="WeekSchedule"/>.</summary>
    public WeekSchedule GetSchedule()
    {
        if (string.IsNullOrEmpty(WorkScheduleJson)) return new WeekSchedule();
        try   { return JsonSerializer.Deserialize<WeekSchedule>(WorkScheduleJson) ?? new WeekSchedule(); }
        catch { return new WeekSchedule(); }
    }

    /// <summary>Serialize và lưu <see cref="WeekSchedule"/> vào <see cref="WorkScheduleJson"/>.</summary>
    public void SetSchedule(WeekSchedule schedule)
        => WorkScheduleJson = JsonSerializer.Serialize(schedule);

    /// <summary>Deserialize ImageUrl JSON thành danh sách URL an toàn.</summary>
    public List<string> GetImageList()
    {
        if (string.IsNullOrEmpty(ImageUrl)) return new();
        try   { return JsonSerializer.Deserialize<List<string>>(ImageUrl) ?? new(); }
        catch { return new(); }
    }
}
