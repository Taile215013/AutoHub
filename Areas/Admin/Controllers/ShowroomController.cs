using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers;

[Area("Admin")]
public class ShowroomController : Controller
{
    private readonly IShowroomRepository _repo;
    private readonly IFileService        _fileService;

    public ShowroomController(IShowroomRepository repo, IFileService fileService)
    {
        _repo        = repo;
        _fileService = fileService;
    }

    // GET /Admin/Showroom
    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());

    // POST /Admin/Showroom/Save — tạo mới hoặc cập nhật (AJAX)
    [HttpPost]
    public async Task<IActionResult> Save(Showroom model, IFormFile? thumbnailFile)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            return Json(new { success = false, message = "Tên showroom không được trống!" });

        try
        {
            bool isNew = model.Id == 0;

            if (thumbnailFile is { Length: > 0 })
                model.ThumbnailImageUrl = await _fileService.UploadThumbnailAsync(thumbnailFile, "showrooms");

            if (isNew)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.IsDeleted = false;
                await _repo.AddAsync(model);
            }
            else
            {
                var existing = await _repo.GetByIdAsync(model.Id);
                if (existing is null)
                    return Json(new { success = false, message = "Không tìm thấy showroom!" });

                existing.Name             = model.Name.Trim();
                existing.Description      = model.Description;
                existing.PhoneNumber      = model.PhoneNumber;
                existing.Email            = model.Email;
                existing.HouseNumber      = model.HouseNumber;
                existing.StreetName       = model.StreetName;
                existing.Ward             = model.Ward;
                existing.District         = model.District;
                existing.City             = model.City;
                existing.Latitude         = model.Latitude;
                existing.Longitude        = model.Longitude;
                existing.OpeningHours     = model.OpeningHours;
                existing.IsActive         = model.IsActive;
                existing.UpdatedAt        = DateTime.UtcNow;

                if (thumbnailFile is { Length: > 0 })
                    existing.ThumbnailImageUrl = model.ThumbnailImageUrl;

                await _repo.UpdateAsync(existing);
                model = existing;
            }

            return Json(new
            {
                success = true,
                message = isNew ? "Thêm showroom thành công!" : "Cập nhật thành công!",
                data    = new
                {
                    id               = model.Id,
                    name             = model.Name,
                    fullAddress      = model.FullAddress,
                    phoneNumber      = model.PhoneNumber,
                    openingHours     = model.OpeningHours,
                    latitude         = model.Latitude,
                    longitude        = model.Longitude,
                    isActive         = model.IsActive,
                    thumbnailImageUrl = model.ThumbnailImageUrl
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Lỗi: " + ex.Message });
        }
    }

    // GET /Admin/Showroom/Get/5
    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s is null) return Json(new { success = false });

        return Json(new
        {
            success   = true,
            data      = new
            {
                id            = s.Id,
                name          = s.Name,
                description   = s.Description,
                phoneNumber   = s.PhoneNumber,
                email         = s.Email,
                houseNumber   = s.HouseNumber,
                streetName    = s.StreetName,
                ward          = s.Ward,
                district      = s.District,
                city          = s.City,
                latitude      = s.Latitude,
                longitude     = s.Longitude,
                openingHours  = s.OpeningHours,
                isActive      = s.IsActive,
                thumbnailImageUrl = s.ThumbnailImageUrl
            }
        });
    }

    // POST /Admin/Showroom/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return Json(new { success = true, message = "Đã xóa showroom!" });
    }
}
