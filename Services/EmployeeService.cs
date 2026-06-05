using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;
    private readonly IFileService _fileService;

    public EmployeeService(IEmployeeRepository repo, IFileService fileService)
    {
        _repo        = repo;
        _fileService = fileService;
    }

    public async Task<Employee> CreateAsync(Employee employee, IFormFile? thumbnail, List<IFormFile>? images)
    {
        employee.ThumbnailImageUrl = await UploadThumbnailAsync(thumbnail);
        employee.ImageUrl          = await UploadImagesAsync(images, existingJson: null);
        employee.CreatedAt         = DateTime.UtcNow;
        employee.IsDeleted         = false;

        await _repo.AddAsync(employee);
        return employee;
    }

    public async Task UpdateAsync(Employee existing, Employee updated, IFormFile? thumbnail, List<IFormFile>? images)
    {
        // Map scalar fields
        existing.FirstName   = updated.FirstName;
        existing.LastName    = updated.LastName;
        existing.Gender      = updated.Gender;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.PhoneNumber = updated.PhoneNumber;
        existing.Email       = updated.Email;
        existing.NationalId  = updated.NationalId;
        existing.HouseNumber = updated.HouseNumber;
        existing.StreetName  = updated.StreetName;
        existing.Ward        = updated.Ward;
        existing.District    = updated.District;
        existing.City        = updated.City;
        existing.HeightCm    = updated.HeightCm;
        existing.WeightKg    = updated.WeightKg;
        existing.Position    = updated.Position;
        existing.BaseSalary  = updated.BaseSalary;
        existing.IsActive    = updated.IsActive;
        existing.Notes       = updated.Notes;
        existing.UpdatedAt   = DateTime.UtcNow;

        // Upload ảnh mới nếu có
        var newThumb = await UploadThumbnailAsync(thumbnail);
        if (!string.IsNullOrEmpty(newThumb))
            existing.ThumbnailImageUrl = newThumb;

        var newImages = await UploadImagesAsync(images, existingJson: existing.ImageUrl);
        if (!string.IsNullOrEmpty(newImages))
            existing.ImageUrl = newImages;

        await _repo.UpdateAsync(existing);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private async Task<string?> UploadThumbnailAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;
        return await _fileService.UploadThumbnailAsync(file, "employees");
    }

    /// <summary>
    /// Upload danh sách ảnh mới, merge vào danh sách cũ (giữ ảnh cũ).
    /// Trả về JSON string hoặc null nếu không có thay đổi.
    /// </summary>
    private async Task<string?> UploadImagesAsync(List<IFormFile>? files, string? existingJson)
    {
        if (files is null || files.Count == 0) return null;

        // Giữ URL cũ
        var urls = new List<string>();
        if (!string.IsNullOrEmpty(existingJson))
        {
            try { urls.AddRange(JsonSerializer.Deserialize<List<string>>(existingJson) ?? new()); }
            catch { /* chuỗi cũ bị corrupt — bỏ qua */ }
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
                urls.Add(await _fileService.UploadImageAsync(file, "employees"));
        }

        return JsonSerializer.Serialize(urls);
    }
}
