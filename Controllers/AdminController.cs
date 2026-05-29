//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using AutoHub.Models.Entities;
//using AutoHub.Repositories;
//using AutoHub.Data;
//using AutoHub.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Http;

//namespace AutoHub.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly IVehicleRepository _vehicleRepository;
//        private readonly IBrandRepository _brandRepository;
//        private readonly ISparePartRepository _sparePartRepository;
//        private readonly IFileService _fileService;
//        private readonly IDashboardService _dashboardService;
//        private readonly ISystemDictionaryService _dictService;

//        public AdminController(
//            IVehicleRepository vehicleRepository,
//            IBrandRepository brandRepository,
//            ISparePartRepository sparePartRepository,
//            IFileService fileService,
//            IDashboardService dashboardService,
//            ISystemDictionaryService dictService)
//        {
//            _vehicleRepository = vehicleRepository;
//            _brandRepository = brandRepository;
//            _sparePartRepository = sparePartRepository;
//            _fileService = fileService;
//            _dashboardService = dashboardService;
//            _dictService = dictService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Dashboard()
//        {
//            ViewBag.TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
//            ViewBag.TotalVehiclesInStock = await _dashboardService.GetTotalVehiclesInStockAsync();
//            ViewBag.TotalBrands = await _dashboardService.GetTotalBrandsAsync();
//            ViewBag.TotalSpareParts = await _dashboardService.GetTotalSparePartsAsync();
//            ViewBag.RecentOrders = await _dashboardService.GetRecentOrdersAsync(5);

//            return View();
//        }

//        public async Task<IActionResult> Index()
//        {
//            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
//            return View(vehicles);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Create()
//        {
//            var brands = await _brandRepository.GetAllWithDetailsAsync();
//            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

//            var dicts = await _dictService.GetAllAsync();
//            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
//            ViewBag.FuelTypes = dicts.Where(d => d.Type == "FuelType").ToList();
//            ViewBag.Transmissions = dicts.Where(d => d.Type == "Transmission").ToList();
//            ViewBag.VehicleColors = dicts.Where(d => d.Type == "VehicleColor").ToList();
//            ViewBag.EngineTypes = dicts.Where(d => d.Type == "EngineType").ToList();
//            ViewBag.BodyStyles = dicts.Where(d => d.Type == "BodyStyle").ToList();

//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
//        {
//            if (uploadedFile != null && uploadedFile.Length > 0)
//            {
//                vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
//            }

//            vehicle.CreatedAt = DateTime.UtcNow;
//            vehicle.IsDeleted = false;

//            await _vehicleRepository.AddAsync(vehicle);

//            if (colors != null && colors.Count > 0)
//            {
//                foreach (var colorName in colors)
//                {
//                    if (!string.IsNullOrWhiteSpace(colorName))
//                    {
//                        var color = new VehicleColor
//                        {
//                            VehicleId = vehicle.Id,
//                            ColorName = colorName.Trim(),
//                            CreatedAt = DateTime.UtcNow,
//                            IsDeleted = false
//                        };
//                        await _vehicleRepository.AddColorAsync(color);
//                    }
//                }
//            }

//            return RedirectToAction(nameof(Index));
//        }

//        [HttpPost]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _vehicleRepository.DeleteAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        [HttpGet]
//public async Task<IActionResult> Edit(int id)
//{
//    var vehicle = await _vehicleRepository.GetByIdAsync(id); // Hoặc hàm tìm kiếm tương đương của bạn
//    if (vehicle == null)
//    {
//        return NotFound();
//    }

//    // Nạp lại các danh mục giống như trang Create để Admin chọn lại nếu muốn
//    ViewBag.VehicleTypes = await _systemDictionaryService.GetByTypeAsync("VehicleType");
//    ViewBag.FuelTypes = await _systemDictionaryService.GetByTypeAsync("FuelType");
//    ViewBag.Transmissions = await _systemDictionaryService.GetByTypeAsync("Transmission");
//    ViewBag.Brands = await _brandRepository.GetAllAsync();
//    ViewBag.EngineTypes = await _systemDictionaryService.GetByTypeAsync("EngineType");
//    ViewBag.BodyStyles = await _systemDictionaryService.GetByTypeAsync("BodyStyle");
//    ViewBag.VehicleColors = await _systemDictionaryService.GetByTypeAsync("VehicleColor");

//    return View(vehicle);
//}

//[HttpPost]
//public async Task<IActionResult> Edit(int id, Vehicle vehicle, List<string> colors, IFormFile? uploadedFile)
//{
//    if (id != vehicle.Id) return BadRequest();

//    try
//    {
//        // 1. Nếu Admin có chọn ảnh mới -> Upload đè lên Cloudinary và lấy link mới
//        if (uploadedFile != null && uploadedFile.Length > 0)
//        {
//            vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
//        }

//        // 2. Gọi Repository để cập nhật dữ liệu xe và mảng màu sắc (colors)
//        await _vehicleRepository.UpdateAsync(vehicle, colors);

//        return RedirectToAction(nameof(Index));
//    }
//    catch (Exception ex)
//    {
//        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);
//        // Load lại ViewBag nếu lỗi để form không bị crash
//        return View(vehicle);
//    }
//}
//==========================================================
//        public async Task<IActionResult> Brands()
//        {
//            var brands = await _brandRepository.GetAllWithDetailsAsync();
//            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
//            return View(brands);
//        }

//        [HttpGet]
//        public async Task<IActionResult> CreateBrand()
//        {
//            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateBrand(Brand brand)
//        {
//            brand.CreatedAt = DateTime.UtcNow;
//            brand.IsDeleted = false;

//            await _brandRepository.AddAsync(brand);
//            return RedirectToAction(nameof(Brands));
//        }

//        [HttpPost]
//        public async Task<IActionResult> DeleteBrand(int id)
//        {
//            await _brandRepository.DeleteAsync(id);
//            return RedirectToAction(nameof(Brands));
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetBrand(int id)
//        {
//            var brand = await _brandRepository.GetByIdAsync(id);
//            if (brand == null)
//            {
//                return Json(new { success = false, message = "Không tìm thấy thương hiệu!" });
//            }
//            return Json(new { 
//                success = true, 
//                data = new {
//                    id = brand.Id,
//                    name = brand.Name,
//                    countryId = brand.CountryId,
//                    isVehicleBrand = brand.IsVehicleBrand,
//                    isPartBrand = brand.IsPartBrand,
//                    isToyBrand = brand.IsToyBrand
//                }
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveBrand(Brand brand)
//        {
//            if (string.IsNullOrWhiteSpace(brand.Name))
//            {
//                return Json(new { success = false, message = "Tên thương hiệu không được để trống!" });
//            }

//            try
//            {
//                bool isNew = (brand.Id == 0);
//                if (isNew)
//                {
//                    brand.CreatedAt = DateTime.UtcNow;
//                    brand.IsDeleted = false;
//                    await _brandRepository.AddAsync(brand);
//                }
//                else
//                {
//                    var existingBrand = await _brandRepository.GetByIdAsync(brand.Id);
//                    if (existingBrand == null)
//                    {
//                        return Json(new { success = false, message = "Thương hiệu không tồn tại hoặc đã bị xóa!" });
//                    }

//                    existingBrand.Name = brand.Name.Trim();
//                    existingBrand.CountryId = brand.CountryId;
//                    existingBrand.IsVehicleBrand = brand.IsVehicleBrand;
//                    existingBrand.IsPartBrand = brand.IsPartBrand;
//                    existingBrand.IsToyBrand = brand.IsToyBrand;

//                    await _brandRepository.UpdateAsync(existingBrand);
//                }

//                var updatedBrand = await _brandRepository.GetByIdAsync(brand.Id);
//                if (updatedBrand == null)
//                {
//                    return Json(new { success = false, message = "Thương hiệu không tồn tại sau khi lưu!" });
//                }

//                return Json(new { 
//                    success = true, 
//                    message = isNew ? "Thêm thương hiệu thành công!" : "Cập nhật thương hiệu thành công!",
//                    data = new {
//                        id = updatedBrand.Id,
//                        name = updatedBrand.Name,
//                        countryName = updatedBrand.Country?.Name ?? "N/A",
//                        isVehicleBrand = updatedBrand.IsVehicleBrand,
//                        isPartBrand = updatedBrand.IsPartBrand,
//                        isToyBrand = updatedBrand.IsToyBrand
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> DeleteBrandAjax(int id)
//        {
//            try
//            {
//                var brand = await _brandRepository.GetByIdAsync(id);
//                if (brand == null)
//                {
//                    return Json(new { success = false, message = "Thương hiệu không tồn tại!" });
//                }

//                await _brandRepository.DeleteAsync(id);
//                return Json(new { success = true, message = "Xóa thương hiệu thành công!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
//            }
//        }


//        public async Task<IActionResult> SpareParts()
//        {
//            var parts = await _sparePartRepository.GetAllWithDetailsAsync();
//            return View(parts);
//        }

//        [HttpGet]
//        public async Task<IActionResult> CreateSparePart()
//        {
//            var brands = await _brandRepository.GetAllWithDetailsAsync();
//            ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();

//            ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateSparePart(SparePart sparePart, IFormFile? ImageFile)
//        {
//            if (sparePart.Price <= sparePart.CostPrice)
//            {
//                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");
//                var brands = await _brandRepository.GetAllWithDetailsAsync();
//                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();

//                ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

//                return View(sparePart);
//            }

//            if (ImageFile != null && ImageFile.Length > 0)
//            {
//                sparePart.ImageUrl = await _fileService.UploadImageAsync(ImageFile, "spareparts");
//            }

//            sparePart.CreatedAt = DateTime.UtcNow;
//            sparePart.IsDeleted = false;

//            await _sparePartRepository.AddAsync(sparePart);
//            return RedirectToAction(nameof(SpareParts));
//        }

//        [HttpPost]
//        public async Task<IActionResult> DeleteSparePart(int id)
//        {
//            await _sparePartRepository.DeleteAsync(id);
//            return RedirectToAction(nameof(SpareParts));
//        }

//    }
//}
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
        // Khai báo các dịch vụ (Services) và kho lưu trữ (Repositories) thông qua Interface
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISparePartRepository _sparePartRepository;
        private readonly IFileService _fileService;
        private readonly IDashboardService _dashboardService;
        private readonly ISystemDictionaryService _dictService;

        /// <summary>
        /// Hàm khởi tạo (Constructor) - Inject các dịch vụ đã được cấu hình trong Program.cs vào Controller
        /// </summary>
        public AdminController(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            ISparePartRepository sparePartRepository,
            IFileService fileService,
            IDashboardService dashboardService,
            ISystemDictionaryService dictService)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _sparePartRepository = sparePartRepository;
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
            // Lấy dữ liệu thống kê tổng hợp từ DashboardService và nạp vào ViewBag để truyền ra View
            ViewBag.TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            ViewBag.TotalVehiclesInStock = await _dashboardService.GetTotalVehiclesInStockAsync();
            ViewBag.TotalBrands = await _dashboardService.GetTotalBrandsAsync();
            ViewBag.TotalSpareParts = await _dashboardService.GetTotalSparePartsAsync();
            ViewBag.RecentOrders = await _dashboardService.GetRecentOrdersAsync(5); // Lấy 5 đơn hàng gần nhất

            return View();
        }

        // =========================================================================
        // QUẢN LÝ KHO XE HƠI (VEHICLES)
        // =========================================================================

        /// <summary>
        /// Trang danh sách quản lý xe - Hiển thị bảng danh mục toàn bộ xe trong kho
        /// URL: /Admin hoặc /Admin/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách xe kèm theo thông tin chi tiết (Brand, Colors, Từ điển phân loại...)
            var vehicles = await _vehicleRepository.GetAllWithDetailsAsync(null, null, null);
            return View(vehicles);
        }

        /// <summary>
        /// Giao diện Thêm xe mới (HTTP GET) - Nạp các danh mục lựa chọn (Dropdown) lên form
        /// URL: /Admin/Create
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // 1. Lấy danh sách thương hiệu và lọc riêng các thương hiệu sản xuất ô tô
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            // 2. Lấy toàn bộ danh mục từ điển hệ thống để gán vào các thẻ <select> trên giao diện
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
            // 1. Nếu Admin có chọn tệp ảnh -> Tiến hành đẩy ảnh lên mây Cloudinary lấy link WebP
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                vehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
            }

            // 2. Thiết lập các thông số hệ thống mặc định cho bản ghi xe mới
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.IsDeleted = false;

            // 3. Lưu thông tin xe vào bảng Vehicles trong cơ sở dữ liệu
            await _vehicleRepository.AddAsync(vehicle);

            // 4. Nếu xe có danh sách màu sắc đi kèm -> Lưu danh sách màu vào bảng liên kết VehicleColors
            if (colors != null && colors.Count > 0)
            {
                foreach (var colorName in colors)
                {
                    if (!string.IsNullOrWhiteSpace(colorName))
                    {
                        var color = new VehicleColor
                        {
                            VehicleId = vehicle.Id, // Link trực tiếp tới ID xe vừa sinh ra ở bước 3
                            ColorName = colorName.Trim(),
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _vehicleRepository.AddColorAsync(color);
                    }
                }
            }

            // Hoàn thành -> Quay trở lại trang danh sách quản lý kho xe
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Giao diện Sửa thông tin xe (HTTP GET) - Tìm xe theo ID và đổ ngược dữ liệu cũ vào Form
        /// URL: /Admin/Edit/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // ĐÃ FIX: Khớp chính xác với tên hàm GetByIdWithDetailsAsync trong EfVehicleRepository của bạn
            var vehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
            if (vehicle == null)
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy xe
            }

            // Nạp lại danh sách thương hiệu xe phục vụ mục lựa chọn lại thương hiệu
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsVehicleBrand).ToList();

            // ĐÃ FIX: Đồng bộ gọi qua biến dịch vụ chính xác là _dictService công cụ
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
            // Bảo mật hệ thống: ID trên Route URL phải trùng khớp với ID đối tượng gửi lên từ Form
            if (id != vehicle.Id) return BadRequest();

            try
            {
                // ĐÃ FIX: Lấy thực thể xe đang sống trực tiếp từ DB ra để chỉnh sửa lõi dữ liệu
                // Giúp giải quyết triệt để lỗi không có hàm Detach và tránh xung đột bộ nhớ tracking của EF
                var existingVehicle = await _vehicleRepository.GetByIdWithDetailsAsync(id);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                // 1. Logic xử lý hình ảnh tối ưu
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // Trường hợp 1: Admin upload ảnh mới -> Thay thế link ảnh cũ thành ảnh WebP mới trên Cloudinary
                    existingVehicle.ImageUrl = await _fileService.UploadImageAsync(uploadedFile, "vehicles");
                }
                // Trường hợp 2: Không up ảnh mới -> Giữ nguyên link cũ trong existingVehicle.ImageUrl không đổi

                // 2. Map (Gán thủ công) toàn bộ dữ liệu thay đổi từ Form đè vào thực thể gốc
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
                existingVehicle.UpdatedAt = DateTime.UtcNow; // Ghi nhận mốc thời gian cập nhật

                // ĐÃ FIX: Gọi hàm UpdateAsync nhận duy nhất 1 tham số theo đúng khai báo trong Repository của bạn
                await _vehicleRepository.UpdateAsync(existingVehicle);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Nếu xảy ra sự cố bất ngờ -> Trả lỗi về ModelState để hiển thị cảnh báo lên giao diện
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật: " + ex.Message);

                // Khôi phục lại toàn bộ dữ liệu ViewBag để Form giao diện không bị sập layout khi load lại
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
        // QUẢN LÝ THƯƠNG HIỆU (BRANDS) - TÍCH HỢP CẢ VIEW TRUYỀN THỐNG VÀ AJAX
        // =========================================================================

        /// <summary>
        /// Trang danh sách thương hiệu đối tác
        /// URL: /Admin/Brands
        /// </summary>
        public async Task<IActionResult> Brands()
        {
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList(); // Nạp danh sách quốc gia xuất xứ
            return View(brands);
        }

        /// <summary>
        /// Giao diện tạo thương hiệu mới dạng trang riêng (HTTP GET)
        /// URL: /Admin/CreateBrand
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateBrand()
        {
            ViewBag.Countries = (await _brandRepository.GetAllCountriesAsync()).ToList();
            return View();
        }

        /// <summary>
        /// Xử lý thêm thương hiệu mới qua form truyền thống (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            brand.CreatedAt = DateTime.UtcNow;
            brand.IsDeleted = false;

            await _brandRepository.AddAsync(brand);
            return RedirectToAction(nameof(Brands));
        }

        /// <summary>
        /// Xóa thương hiệu qua form truyền thống (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            await _brandRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Brands));
        }

        /// <summary>
        /// API lấy nhanh thông tin Thương hiệu bằng Ajax dạng JSON (Dùng cho Modal Popup sửa nhanh)
        /// URL: /Admin/GetBrand/{id}
        /// </summary>
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

        /// <summary>
        /// API Tích hợp: Thêm mới HOẶC Cập nhật thương hiệu bằng xử lý ngầm Ajax gửi dữ liệu JSON
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveBrand(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                return Json(new { success = false, message = "Tên thương hiệu không được để trống!" });
            }

            try
            {
                // Nhận biết: Nếu Id gửi lên bằng 0 -> Hành động Thêm mới. Ngược lại -> Hành động Sửa.
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

                    // Map dữ liệu mới đè lên dữ liệu cũ đang được tracking trong DB
                    existingBrand.Name = brand.Name.Trim();
                    existingBrand.CountryId = brand.CountryId;
                    existingBrand.IsVehicleBrand = brand.IsVehicleBrand;
                    existingBrand.IsPartBrand = brand.IsPartBrand;
                    existingBrand.IsToyBrand = brand.IsToyBrand;

                    await _brandRepository.UpdateAsync(existingBrand);
                }

                // Lấy bản ghi thương hiệu sau khi cập nhật kèm dữ liệu quan hệ bảng Country để hiển thị lại lên hàng của Table
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

        /// <summary>
        /// API Xóa thương hiệu ngầm bằng Ajax không làm tải lại toàn bộ trang web
        /// </summary>
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

        /// <summary>
        /// Trang hiển thị danh sách quản lý linh kiện, phụ tùng ô tô
        /// URL: /Admin/SpareParts
        /// </summary>
        public async Task<IActionResult> SpareParts()
        {
            var parts = await _sparePartRepository.GetAllWithDetailsAsync();
            return View(parts);
        }

        /// <summary>
        /// Giao diện Thêm linh kiện phụ tùng mới (HTTP GET)
        /// URL: /Admin/CreateSparePart
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateSparePart()
        {
            // Lấy danh sách thương hiệu chuyên sản xuất linh kiện, phụ tùng
            var brands = await _brandRepository.GetAllWithDetailsAsync();
            ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();

            // Nạp phân loại nhóm ngành hàng phụ tùng (Ví dụ: Động cơ, Lốp xe, Má phanh...)
            ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

            return View();
        }

        /// <summary>
        /// Xử lý lưu dữ liệu Thêm linh kiện phụ tùng mới (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSparePart(SparePart sparePart, IFormFile? ImageFile)
        {
            // Nghiệp vụ logic kinh doanh: Giá bán ra thị trường bắt buộc phải lớn hơn giá vốn nhập kho
            if (sparePart.Price <= sparePart.CostPrice)
            {
                ModelState.AddModelError("Price", "Giá bán lẻ ra thị trường phải lớn hơn giá gốc nhập vào!");

                // Khôi phục lại dữ liệu ViewBag để Form không bị crash khi load lại
                var brands = await _brandRepository.GetAllWithDetailsAsync();
                ViewBag.Brands = brands.Where(b => b.IsPartBrand).ToList();
                ViewBag.Categories = await _dictService.GetDictionariesByTypeAsync("SparePartCategory");

                return View(sparePart);
            }

            // Nếu phụ tùng có đính kèm ảnh -> Đẩy lên mây Cloudinary lưu vào thư mục riêng biệt "spareparts"
            if (ImageFile != null && ImageFile.Length > 0)
            {
                sparePart.ImageUrl = await _fileService.UploadImageAsync(ImageFile, "spareparts");
            }

            sparePart.CreatedAt = DateTime.UtcNow;
            sparePart.IsDeleted = false;

            await _sparePartRepository.AddAsync(sparePart);
            return RedirectToAction(nameof(SpareParts));
        }

        /// <summary>
        /// Xóa phụ tùng khỏi hệ thống dựa theo ID (HTTP POST)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteSparePart(int id)
        {
            await _sparePartRepository.DeleteAsync(id);
            return RedirectToAction(nameof(SpareParts));
        }
    } // Hết lớp AdminController đóng ngoặc chuẩn
} // Hết Không gian tên Namespace đóng ngoặc chuẩn