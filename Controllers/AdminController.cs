using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly AppDbContext _context;

        public AdminController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            ISparePartRepository sparePartRepository,
            AppDbContext context)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _sparePartRepository = sparePartRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Tổng doanh thu (Tổng TotalAmount của các Orders)
            var totalRevenue = await _context.Orders
                .Where(o => !o.IsDeleted)
                .SumAsync(o => o.TotalAmount);

            // 2. Tổng số xe trong kho (Tổng Quantity của tất cả Vehicles)
            var totalVehiclesInStock = await _context.Vehicles
                .Where(v => !v.IsDeleted)
                .SumAsync(v => v.Quantity);

            // 3. Tổng số thương hiệu
            var totalBrands = await _context.Brands
                .Where(b => !b.IsDeleted)
                .CountAsync();

            // 4. Tổng số linh kiện/phụ tùng
            var totalSpareParts = await _context.SpareParts
                .Where(p => !p.IsDeleted)
                .CountAsync();

            // 5. Danh sách 5 đơn hàng gần nhất
            var recentOrders = await _context.Orders
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalVehiclesInStock = totalVehiclesInStock;
            ViewBag.TotalBrands = totalBrands;
            ViewBag.TotalSpareParts = totalSpareParts;
            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
            return View(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            var dicts = await _context.SystemDictionaries.ToListAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, List<string> colors)
        {
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.IsDeleted = false;

            await _vehicleRepository.AddAsync(vehicle);

            if (colors != null && colors.Count > 0)
            {
                foreach (var colorName in colors)
                {
                    if (!string.IsNullOrWhiteSpace(colorName))
                    {
                        var color = new VehicleColor
                        {
                            VehicleId = vehicle.Id,
                            ColorName = colorName.Trim(),
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _vehicleRepository.AddColorAsync(color);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Brands()
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
            return RedirectToAction(nameof(Brands));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Brands));
        }

        [HttpGet]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thương hiệu!" });
            }
            return Json(new { 
                success = true, 
                data = new {
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

                return Json(new { 
                    success = true, 
                    message = isNew ? "Thêm thương hiệu thành công!" : "Cập nhật thương hiệu thành công!",
                    data = new {
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


        public async Task<IActionResult> SpareParts()
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();
            return View(parts);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSparePart()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();

            ViewBag.Categories = await _context.SystemDictionaries
                .Where(d => d.Type == "SparePartCategory")
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSparePart(SparePart sparePart)
        {
            if (sparePart.Price <= sparePart.CostPrice)
            {
                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");
                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();

                ViewBag.Categories = await _context.SystemDictionaries
                    .Where(d => d.Type == "SparePartCategory")
                    .ToListAsync();

                return View(sparePart);
            }

            sparePart.CreatedAt = DateTime.UtcNow;
            sparePart.IsDeleted = false;

            await _sparePartRepository.AddAsync(sparePart);
            return RedirectToAction(nameof(SpareParts));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSparePart(int id)
        {
            await _sparePartRepository.DeleteAsync(id);
            return RedirectToAction(nameof(SpareParts));
        }

    }
}
