using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ISystemDictionaryService _dictService;
        private readonly IMasterDataRepository _masterDataRepository;

        public ServiceController(
            IServiceRepository serviceRepository,
            ISystemDictionaryService dictService,
            IMasterDataRepository masterDataRepository)
        {
            _serviceRepository = serviceRepository;
            _dictService = dictService;
            _masterDataRepository = masterDataRepository;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _serviceRepository.GetAllAsync();
            var dicts = await _dictService.GetAllAsync();
            ViewBag.VehicleTypes = dicts.Where(d => d.Type == "VehicleType").ToList();
            ViewBag.ServiceCategories = await _masterDataRepository.GetCategoriesByTypeAsync("Service");
            return View(services);
        }

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
                    categoryId = service.CategoryId,
                    categoryName = service.Category?.Name,
                    basePrice = service.BasePrice,
                    requiresQuote = service.RequiresQuote,
                    isActive = service.IsActive
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveService(Service service, string? newCategory)
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
                    if (!service.CategoryId.HasValue && !string.IsNullOrWhiteSpace(newCategory))
                    {
                        var category = await _masterDataRepository.FindOrCreateCategoryAsync("Service", newCategory);
                        service.CategoryId = category?.Id;
                    }

                    service.CreatedAt = DateTime.UtcNow;
                    service.IsDeleted = false;
                    await _serviceRepository.AddAsync(service);
                }
                else
                {
                    var existing = await _serviceRepository.GetByIdAsync(service.Id);
                    if (existing == null)
                    {
                        return Json(new { success = false, message = "Dịch vụ không tồn tại hoặc đã bị xóa!" });
                    }

                    existing.ServiceName = service.ServiceName.Trim();
                    if (!service.CategoryId.HasValue && !string.IsNullOrWhiteSpace(newCategory))
                    {
                        var category = await _masterDataRepository.FindOrCreateCategoryAsync("Service", newCategory);
                        service.CategoryId = category?.Id;
                    }
                    existing.CategoryId = service.CategoryId;
                    existing.VehicleType = service.VehicleType;
                    existing.BasePrice = service.RequiresQuote ? null : service.BasePrice;
                    existing.RequiresQuote = service.RequiresQuote;
                    existing.IsActive = service.IsActive;
                    existing.UpdatedAt = DateTime.UtcNow;

                    await _serviceRepository.UpdateAsync(existing);
                }

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
                        categoryId = saved.CategoryId,
                        categoryName = saved.Category?.Name,
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
    }
}
