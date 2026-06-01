using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandRepository _brandRepository;

        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
            return View(brands);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBrand()
        {
            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            brand.CreatedAt = DateTime.UtcNow;
            brand.IsDeleted = false;

            await _brandRepository.AddAsync(brand);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thương hiệu!" });
            }
            return Json(new
            {
                success = true,
                data = new
                {
                    id = brand.Id,
                    name = brand.Name,
                    countryId = brand.CountryId,
                    isVehicleBrand = brand.IsVehicleBrand,
                    isPartBrand = brand.IsPartBrand,
                    isToyBrand = brand.IsToyBrand
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBrand(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                return Json(new { success = false, message = "Tên thương hiệu không được để trống!" });
            }

            try
            {
                bool isNew = (brand.Id == 0);
                if (isNew)
                {
                    brand.CreatedAt = DateTime.UtcNow;
                    brand.IsDeleted = false;
                    await _brandRepository.AddAsync(brand);
                }
                else
                {
                    var existingBrand = await _brandRepository.GetByIdAsync(brand.Id);
                    if (existingBrand == null)
                    {
                        return Json(new { success = false, message = "Thương hiệu không tồn tại hoặc đã bị xóa!" });
                    }

                    existingBrand.Name = brand.Name.Trim();
                    existingBrand.CountryId = brand.CountryId;
                    existingBrand.IsVehicleBrand = brand.IsVehicleBrand;
                    existingBrand.IsPartBrand = brand.IsPartBrand;
                    existingBrand.IsToyBrand = brand.IsToyBrand;

                    await _brandRepository.UpdateAsync(existingBrand);
                }

                var updatedBrand = await _brandRepository.GetByIdAsync(brand.Id);
                if (updatedBrand == null)
                {
                    return Json(new { success = false, message = "Thương hiệu không tồn tại sau khi lưu!" });
                }

                return Json(new
                {
                    success = true,
                    message = isNew ? "Thêm thương hiệu thành công!" : "Cập nhật thương hiệu thành công!",
                    data = new
                    {
                        id = updatedBrand.Id,
                        name = updatedBrand.Name,
                        countryName = updatedBrand.Country?.Name ?? "N/A",
                        isVehicleBrand = updatedBrand.IsVehicleBrand,
                        isPartBrand = updatedBrand.IsPartBrand,
                        isToyBrand = updatedBrand.IsToyBrand
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrandAjax(int id)
        {
            try
            {
                var brand = await _brandRepository.GetByIdAsync(id);
                if (brand == null)
                {
                    return Json(new { success = false, message = "Thương hiệu không tồn tại!" });
                }

                await _brandRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa thương hiệu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
