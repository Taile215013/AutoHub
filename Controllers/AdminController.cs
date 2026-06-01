using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Data;
using AutoHub.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace AutoHub.Controllers
{
    /// <summary>
    /// Controller điều hướng toàn bộ khu vực quản trị (Admin Dashboard) của AutoHub
    /// </summary>
    public class AdminController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFileService _fileService;
        private readonly IDashboardService _dashboardService;
        private readonly ISystemDictionaryService _dictService;

        /// <summary>
        /// Constructor - Inject tất cả dịch vụ cần thiết vào Controller
        /// </summary>
        public AdminController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            ISparePartRepository sparePartRepository,
            IServiceRepository serviceRepository,
            IFileService fileService,
            IDashboardService dashboardService,
            ISystemDictionaryService dictService)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _sparePartRepository = sparePartRepository;
            _serviceRepository = serviceRepository;
            _fileService = fileService;
            _dashboardService = dashboardService;
            _dictService = dictService;
        }

        // =========================================================================
        // QUẢN LÝ BẢNG ĐIỀU KHIỂN CHÍNH (DASHBOARD)
        // =========================================================================

        /// <summary>
        /// Trang chủ quản trị - Hiển thị các thông số thống kê tài chính và báo cáo nhanh
        /// URL: /Admin/Dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            ViewBag.TotalVehiclesInStock = await _dashboardService.GetTotalVehiclesInStockAsync();
            ViewBag.TotalBrands = await _dashboardService.GetTotalBrandsAsync();
            ViewBag.TotalSpareParts = await _dashboardService.GetTotalSparePartsAsync();
            ViewBag.RecentOrders = await _dashboardService.GetRecentOrdersAsync(5);

            return View();
        }

        // =========================================================================
        // QUẢN LÝ KHO XE HƠI (VEHICLES)
        // =========================================================================

        /// <summary>
        /// Trang danh sách quản lý xe
        /// URL: /Admin hoặc /Admin/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
            return View(vehicles);
        }

        /// <summary>
        /// Giao diện Thêm xe mới (HTTP GET)
        /// URL: /Admin/Create
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

            return View();
        }

        /// <summary>
        /// Xử lý lưu dữ liệu Thêm xe mới (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
            }

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

        /// <summary>
        /// Giao diện Sửa thông tin xe (HTTP GET)
        /// URL: /Admin/Edit/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

            return View(vehicle);
        }

        /// <summary>
        /// Xử lý cập nhật thông tin xe sau khi sửa (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
        {
            if (id != vehicle.Id) return BadRequest();

            try
            {
                var existingVehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    existingVehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
                }

                existingVehicle.Name = vehicle.Name;
                existingVehicle.BrandId = vehicle.BrandId;
                existingVehicle.VehicleType = vehicle.VehicleType;
                existingVehicle.BodyStyle = vehicle.BodyStyle;
                existingVehicle.FuelType = vehicle.FuelType;
                existingVehicle.EngineType = vehicle.EngineType;
                existingVehicle.Transmission = vehicle.Transmission;
                existingVehicle.PurchasePrice = vehicle.PurchasePrice;
                existingVehicle.CurrentPrice = vehicle.CurrentPrice;
                existingVehicle.Quantity = vehicle.Quantity;
                existingVehicle.Description = vehicle.Description;
                existingVehicle.UpdatedAt = DateTime.UtcNow;

                await _vehicleRepository.UpdateAsync(existingVehicle);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);

                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

                var dicts = await _dictService.GetAllAsync();
                ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
                ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
                ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
                ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
                ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
                ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

                return View(vehicle);
            }
        }

        /// <summary>
        /// Xử lý Xóa xe khỏi hệ thống (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _vehicleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // QUẢN LÝ THƯƠNG HIỆU (BRANDS)
        // =========================================================================

        /// <summary>
        /// Trang danh sách thương hiệu đối tác
        /// URL: /Admin/Brands
        /// </summary>
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

        // =========================================================================
        // QUẢN LÝ KHO PHỤ TÙNG Ô TÔ (SPARE PARTS)
        // =========================================================================

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
            ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSparePart(SparePart sparePart, IFormFile? ImageFile)
        {
            if (sparePart.Price <= sparePart.CostPrice)
            {
                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");

                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
                ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

                return View(sparePart);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                sparePart.ImageUrl = await _fileService.UploadImageAsync(ImageFile, "spareparts");
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

        // =========================================================================
        // QUẢN LÝ DỊCH VỤ (SERVICES)
        // =========================================================================

        /// <summary>
        /// Trang quản lý dịch vụ - hiển thị toàn bộ danh sách dịch vụ
        /// URL: /Admin/Services
        /// </summary>
        public async Task<IActionResult> Services()
        {
            var services = await _serviceRepository.GetAllAsync();
            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            return View(services);
        }

        /// <summary>
        /// API lấy thông tin 1 dịch vụ theo ID (dùng cho modal sửa)
        /// URL: /Admin/GetService/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetService(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dịch vụ!" });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    id = service.Id,
                    serviceName = service.ServiceName,
                    vehicleType = service.VehicleType,
                    basePrice = service.BasePrice,
                    requiresQuote = service.RequiresQuote,
                    isActive = service.IsActive
                }
            });
        }

        /// <summary>
        /// API Thêm mới HOẶC Cập nhật dịch vụ bằng AJAX
        /// URL: /Admin/SaveService
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveService(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.ServiceName))
            {
                return Json(new { success = false, message = "Tên dịch vụ không được để trống!" });
            }

            try
            {
                bool isNew = (service.Id == 0);
                service.VehicleType = service.VehicleType ?? "";

                if (isNew)
                {
                    // Thêm mới
                    service.CreatedAt = DateTime.UtcNow;
                    service.IsDeleted = false;
                    await _serviceRepository.AddAsync(service);
                }
                else
                {
                    // Cập nhật - fetch từ DB trước để tránh lỗi EF tracking
                    var existing = await _serviceRepository.GetByIdAsync(service.Id);
                    if (existing == null)
                    {
                        return Json(new { success = false, message = "Dịch vụ không tồn tại hoặc đã bị xóa!" });
                    }

                    existing.ServiceName = service.ServiceName.Trim();
                    existing.VehicleType = service.VehicleType;
                    existing.BasePrice = service.RequiresQuote ? null : service.BasePrice;
                    existing.RequiresQuote = service.RequiresQuote;
                    existing.IsActive = service.IsActive;
                    existing.UpdatedAt = DateTime.UtcNow;

                    await _serviceRepository.UpdateAsync(existing);
                }

                // Lấy lại record vừa lưu để trả về cho UI cập nhật row
                var saved = isNew
                    ? (await _serviceRepository.GetAllAsync()).OrderByDescending(s => s.Id).First()
                    : await _serviceRepository.GetByIdAsync(service.Id);

                return Json(new
                {
                    success = true,
                    message = isNew ? "Thêm dịch vụ thành công!" : "Cập nhật dịch vụ thành công!",
                    data = new
                    {
                        id = saved!.Id,
                        serviceName = saved.ServiceName,
                        vehicleType = saved.VehicleType,
                        basePrice = saved.BasePrice,
                        requiresQuote = saved.RequiresQuote,
                        isActive = saved.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        /// <summary>
        /// API Xóa mềm dịch vụ bằng AJAX
        /// URL: /Admin/DeleteServiceAjax
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteServiceAjax(int id)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return Json(new { success = false, message = "Dịch vụ không tồn tại!" });
                }

                await _serviceRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }

    } // Hết lớp AdminController
} // Hết Namespace